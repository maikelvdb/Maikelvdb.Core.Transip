using Maikelvdb.Core.Transip.Api.Interfaces;
using System.Collections.Generic;

namespace Maikelvdb.Core.Transip.Api.Models.Products
{
    public class ProductsResponse : IResponseResult
    {
        public ProductsResponse()
        {
            Products = new Dictionary<string, IList<Product>>();
        }

        public IDictionary<string, IList<Product>> Products { get; set; }
    }
}
