using Maikelvdb.Core.Transip.ConsoleApp.Helpers;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Maikelvdb.Core.Transip.ConsoleApp
{
    /// <summary>
    /// https://github.com/WouterJanson/TransIP.NET/blob/master/src/TransIp.Api/ClientBase.cs
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string privateKey = @"-----BEGIN PRIVATE KEY-----
MIIEwAIBADANBgkqhkiG9w0BAQEFAASCBKowggSmAgEAAoIBAQCj+epEFOm2W1Yc
JCklqj+cFk3xc43Gp/pYyEfWVBGFah7njtXDAqS+Q0n9H52qoNl7Ee4KkWg9adeP
JRcuTIyFVMZ1G69doChZ6Ww0vQJCkmFeeGZz6V+n2BTVl6SwMTmZYwE9SfmtAZCQ
baOcHJpCBcKFumUuHMEonnZCI7Eg1c7XcKfyLG30q9QJ84UxSj5Cnx+xqdX+mcu8
7bv4+itiSwnr5HiEpmxwX4CogIvGYzzgMXwNTbJIkUxyW8Lt8EUKCrvf37uT1+4T
pDb1tJl+oXk/8kTQZ6diETzrBy1oygoLQ9A8eZdcL2r+Gx6FpWzlXRD61XSLLzpe
PmcwpvnVAgMBAAECggEBAKMQH9rcQR3K7nLFsvV1vBPoSlJV6yMmYFpxsJ5+s33M
7javX/uxYUqjd6x0CEJp+lvclK+FsfeAjZAX/llYXlUo3MIQae/dGMNw0N4RJ3I0
Rwe/C4MxDKLIys8tIeTkzrauMclVCyj0aWJ25UCVYQFO4QZDOeMFTUCrdEaJX9eZ
r2XCN54rc8BNTMYsteoMk6wPxY9QxEj1AlIDoPZvoVGpOpj/SVB57AponONsvxHQ
HiGC1q/prZkI1hHGf10x9/IMKmBGpxBJY2CWfJgWS1SzhwgU1h9jv89Ez+tvm5JW
9C4b9XY/n+CYEyZcwN1P7sjrApzWCf5xDzclimtV5AECgYEA1XFlYj6vJi6tJYgc
p7uo6EPNXnc1Tiu3FsCphQ1BAubApUFZDEQbSsOZ3OcVsHC79KB9EJILux8sp56u
RMn5vaV9ml8LKQB7QfTHnZ2SClZPRT882V0Tdk17LWLTWDhfAx6VMR5YDOI/4Jof
HrmqQnyqWrqwc+VmyJ1EK4uSuEECgYEAxKugmKUJqXUiqJXx9+h6JIve3kCi/kma
NqGVU0X441fxOqCLUcso/Z0qzEsE39V93Wor1RKk2kGUmemMBWiay9HUZmpfnO8m
u8MgTGwwLxzC6IXGweYFZdiYI0wOyYHcTXfsd3uRT0WJFPaUiXWnn4ZfG9jBWB2r
23MOryhyvJUCgYEAh2zxdMps+v72oS+hbtNksioC7gMY8UZPXsJTTLZCH4MljDTQ
OTYlUuSUhVKhsld1Chuh/peLleiLvraxv7efG7Ma5I2VWSiTDCAxX1IQzTpCBZ/A
DfI+XPuAQiXIDtuFfUc0RfwIxfPvr6RaRnZrDtBmjjaRqpdNBdCy5iYiq4ECgYEA
jy6oiBk3dJDb/8LS2/r72dmfE04ZnGo3hOiUz4CU1+cxr84sQAtZt3KSMxO248MJ
lIZ0jPa64E94gal/kHx8nkEQktOE8rtGBtLjk9/8IICfAixK7OWrLl/HO4NnDJun
qRrTJJXoz76/M2zl7UGkHMb2PU7MTHuln+ofWbRn/BUCgYEApPpJDX4jgcYM05vI
QdKDrbP0jV1SAi5ZeZScW1AUaHpMXyjzoJwJUnIbPBhBomHFZY8IVewmXaEehGvd
W6+bAc7vL84Ca9Xy0tIgip89BrdQLgTVESjUOvSPvmDaiUPQ2BTVLV2uxjXKQBOp
NfOP/1rrUq/90l3VS+vnPejLhd0=
-----END PRIVATE KEY-----";

            var body = new Body
            {
                Login = "maikelvdb",
                Nonce = Guid.NewGuid().ToString().Substring(0, 2),
                Read_only = false,
                Expiration_time = "30 minutes",
                Label = "",
                Global_key = false
            };

            var bodyJson = JsonConvert.SerializeObject(body);

            //var hash = SHA512_ComputeHash(bodyJson, privateKey);
            var hash = EncryptString(privateKey, bodyJson);
            
            Console.WriteLine(hash);

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Signature", hash);
            http.BaseAddress = new Uri("https://api.transip.nl/v6/");

            var buffer = Encoding.UTF8.GetBytes(bodyJson);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var resp = http.PostAsync("auth", byteContent).GetAwaiter().GetResult();
            var cont = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Console.ReadKey();
        }

        public static string EncryptString(string key, string body)
        {
            return EncryptionHelper.Encode(EncryptionHelper.Sign(key, body));
                
               // new object[]
               //{
               // new KeyValuePair<string, string>("login", "maikelvdb"),
               // new KeyValuePair<string, string>("nonce", "1"),
               // new KeyValuePair<string, string>("read_only", false.ToString()),
               // new KeyValuePair<string, string>("expiration_time", "30 minutes"),
               // new KeyValuePair<string, string>("Label", ""),
               // new KeyValuePair<string, string>("global_key", false.ToString())
               //}));
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }


    public class Body
    {
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        
        [JsonProperty("read_only")]
        public bool Read_only { get; set; }

        [JsonProperty("expiration_time")]
        public string Expiration_time { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("global_key")]
        public bool Global_key { get; set; }
    }
}
