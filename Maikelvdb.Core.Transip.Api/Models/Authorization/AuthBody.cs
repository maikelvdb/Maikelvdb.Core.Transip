using Newtonsoft.Json;

namespace Maikelvdb.Core.Transip.Api.Models.Authorization
{
    public class AuthBody
    {
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("read_only")]
        public bool ReadOnly { get; set; }

        [JsonProperty("expiration_time")]
        public string ExpirationTime { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("global_key")]
        public bool GlobalKey { get; set; }
    }
}
