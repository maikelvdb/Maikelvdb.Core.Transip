using Newtonsoft.Json;
using System;

namespace Maikelvdb.Core.Transip.Api.Exceptions
{
    public class ApiErrorException : Exception
    {
        public ApiErrorException(string error): base(error)
        { }

        public static ApiErrorException Create(string apiError)
        {
            var errorModel = JsonConvert.DeserializeObject<ErrorModel>(apiError);

            return new ApiErrorException(errorModel.Error);
        }

        private class ErrorModel
        {
            public string Error { get; set; }
        }
    }
}
