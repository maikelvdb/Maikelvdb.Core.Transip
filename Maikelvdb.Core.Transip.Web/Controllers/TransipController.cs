using Maikelvdb.Core.Transip.Api.Interfaces;
using Maikelvdb.Core.Transip.Api.Models.Products;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Maikelvdb.Core.Transip.Web.Controllers
{
    [Route("api/[controller]")]
    public class TransipController : ControllerBase
    {
        private readonly ITransipApi _api;

        public TransipController(ITransipApi api)
        {
            _api = api;
        }

        [HttpGet("products")]
        public async Task<ProductsResponse> GetProducts()
        {
            return await _api.ProductsAsync();
        }
    }
}
