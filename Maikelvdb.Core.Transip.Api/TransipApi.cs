using Maikelvdb.Core.Transip.Api.Constants;
using System.Threading.Tasks;
using Maikelvdb.Core.Transip.Api.Interfaces;
using Maikelvdb.Core.Transip.Api.Framework;
using Maikelvdb.Core.Transip.Api.Models.Products;
using Maikelvdb.Core.Transip.Api.Models.Options;

namespace Maikelvdb.Core.Transip.Api
{
    public class TransipApi : TransipBaseApi, ITransipApi
    {
        public TransipApi(TransipApiOptions options) : base(options) { }

        public async Task<ProductsResponse> ProductsAsync()
        {
            return await ExecuteAsync(new ExecuteRequest<ProductsResponse>
            {
                Type = Enums.RequestType.Get,
                EndPointPath = TransipConstants.Urls.Products
            });
        }
    }
}
