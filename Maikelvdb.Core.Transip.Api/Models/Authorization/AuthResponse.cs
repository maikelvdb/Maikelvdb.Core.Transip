using Maikelvdb.Core.Transip.Api.Interfaces;

namespace Maikelvdb.Core.Transip.Api.Models.Authorization
{
    public class AuthResponse : IResponseResult
    {
        public string Token { get; set; }
    }
}
