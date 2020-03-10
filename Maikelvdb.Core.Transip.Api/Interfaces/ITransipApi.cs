using Maikelvdb.Core.Transip.Api.Models.Products;
using System;
using System.Threading.Tasks;

namespace Maikelvdb.Core.Transip.Api.Interfaces
{

    public interface ITransipApi
    {
        Task<ProductsResponse> ProductsAsync();
    }
}
