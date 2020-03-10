using Maikelvdb.Core.Transip.Api.Enums;
using Maikelvdb.Core.Transip.Api.Interfaces;
using System.Collections.Generic;

namespace Maikelvdb.Core.Transip.Api.Framework
{
    public class ExecuteRequest<TResponse> where TResponse : class, IResponseResult
    {
        public ExecuteRequest()
        {
            Headers = new Dictionary<string, string>();
        }

        public RequestType Type { get; set; }
        public string EndPointPath { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public object Data { get; set; }
    }
}
