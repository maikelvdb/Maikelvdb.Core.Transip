using System;

namespace Maikelvdb.Core.Transip.Api.Models.Authorization
{
    public class AuthModel
    {
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public AuthBody Body { get; set; }
    }
}
