using Maikelvdb.Core.Transip.Api.Constants;
using Maikelvdb.Core.Transip.Api.Exceptions;
using Maikelvdb.Core.Transip.Api.Framework;
using Maikelvdb.Core.Transip.Api.Helpers;
using Maikelvdb.Core.Transip.Api.Interfaces;
using Maikelvdb.Core.Transip.Api.Models.Authorization;
using Maikelvdb.Core.Transip.Api.Models.Options;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Maikelvdb.Core.Transip.Api
{
    public abstract class TransipBaseApi : IDisposable
    {
        private readonly HttpClient _client;
        private readonly TransipApiOptions _options;

        private string _signature;
        private RegistryKey _regKey;
        private AuthModel _authorization;

        public TransipBaseApi(TransipApiOptions options)
        {
            _options = options;
            ValidateOptions(_options);

            _client = new HttpClient
            {
                BaseAddress = new Uri(TransipConstants.BaseUrl)
            };

            CheckRegistryValues();
        }

        private void CheckRegistryValues()
        {
            _regKey = Registry.CurrentUser.OpenSubKey(ApplicationConstants.RegistrySubKey, true);
            if (_regKey == null)
            {
                _regKey = Registry.CurrentUser.CreateSubKey(ApplicationConstants.RegistrySubKey, true);
            }
            else
            {
                var auth = _regKey.GetValue(ApplicationConstants.RegistryKey);
                if (auth != null)
                {
                    _authorization = JsonConvert.DeserializeObject<AuthModel>(auth.ToString());
                }
            }
        }

        protected async Task<TResponse> ExecuteAsync<TResponse>(ExecuteRequest<TResponse> info)
            where TResponse : class, IResponseResult
        {
            if (info.EndPointPath != TransipConstants.Urls.Auth)
                await SetAuthHeaderAsync();

            SetHeaders(info);

            TResponse response = null;
            response = info.Type switch
            {
                Enums.RequestType.Get => await ExecuteGetAsync(info),
                Enums.RequestType.Post => await ExecutePostAsync(info),
                _ => throw new NotSupportedException("Unsupported request type")
            };

            CleanHeaders(info);

            return response;
        }

        private void SetHeaders<TResponse>(ExecuteRequest<TResponse> info) where TResponse : class, IResponseResult
        {
            foreach (var value in info.Headers)
            {
                _client.DefaultRequestHeaders.Add(value.Key, value.Value);
            }
        }

        private void CleanHeaders<TResponse>(ExecuteRequest<TResponse> info) where TResponse : class, IResponseResult
        {
            foreach (var value in info.Headers)
            {
                _client.DefaultRequestHeaders.Remove(value.Key);
            }
        }

        private async Task<TResponse> ExecuteGetAsync<TResponse>(ExecuteRequest<TResponse> info) where TResponse : class, IResponseResult
        {
            var request = await _client.GetAsync(CreateEndpoint(info));
            return await CreateResult(info, request);
        }

        private async Task<TResponse> ExecutePostAsync<TResponse>(ExecuteRequest<TResponse> info) where TResponse : class, IResponseResult
        {
            var json = string.Empty;
            if (info.Data != null)
                json = JsonConvert.SerializeObject(info.Data);

            var buffer = Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var request = await _client.PostAsync(CreateEndpoint(info), byteContent);
            return await CreateResult(info, request);
        }

        private async Task<TResponse> CreateResult<TResponse>(ExecuteRequest<TResponse> info, HttpResponseMessage request) where TResponse : class, IResponseResult
        {
            if (request.IsSuccessStatusCode)
            {
                var content = await request.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TResponse>(content);
            }

            var errorContent = await request.Content.ReadAsStringAsync();
            throw ApiErrorException.Create(errorContent);
        }

        private async Task SetAuthHeaderAsync()
        {

            if (_authorization == null || _authorization.ExpireDate < DateTime.UtcNow)
            {
                _authorization = new AuthModel
                {
                    Body = new AuthBody
                    {
                        Login = "maikelvdb",
                        Nonce = Guid.NewGuid().ToString().Substring(0, 16),
                        ReadOnly = false,
                        ExpirationTime = "30 minutes",
                        Label = "",
                        GlobalKey = true
                    }
                };

                if (_signature == null)
                {
                    var key = await File.ReadAllTextAsync(_options.PrivateKeyPath);
                    _signature = EncryptionHelper.Sign(key, JsonConvert.SerializeObject(_authorization.Body));
                }

                var auth = await AuthAsync();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(auth.Token);

                _authorization.ExpireDate = token.ValidTo;
                _authorization.Token = auth.Token;

                _regKey.SetValue(ApplicationConstants.RegistryKey, JsonConvert.SerializeObject(_authorization));
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authorization.Token);
        }

        private async Task<AuthResponse> AuthAsync()
        {
            var result = await ExecuteAsync(new ExecuteRequest<AuthResponse>
            {
                Type = Enums.RequestType.Post,
                EndPointPath = TransipConstants.Urls.Auth,
                Headers = new Dictionary<string, string> {
                    { "Signature", _signature }
                },
                Data = _authorization.Body
            });

            return result;
        }

        private string CreateEndpoint<TResponse>(ExecuteRequest<TResponse> info) where TResponse : class, IResponseResult
        {
            if (_options.IsTest)
                return $"{info.EndPointPath}{(info.EndPointPath.Contains("?") ? "&test=1" : "?test=1")}";

            return info.EndPointPath;
        }
        private void ValidateOptions(TransipApiOptions options)
        {
            if (!options.HasPrivateKeyPath)
                throw new Exception("Private key path is not set");

            if (!File.Exists(options.PrivateKeyPath))
                throw new FileNotFoundException($"Private key file is not found in: {options.PrivateKeyPath}");
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
