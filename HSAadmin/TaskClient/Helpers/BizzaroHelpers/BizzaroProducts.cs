using System;
using System.Threading.Tasks;
using HsaServiceDtos;
using HSAManager.Helpers.BizzaroHelpers;
using Microsoft.Identity.Client;
using RestSharp;

namespace TaskClient.Helpers.BizzaroHelpers
{
    public class BizzaroProducts : AbstractBizzaroActions
    {
        public BizzaroProducts(AuthenticationResult authenticationResult) : base(authenticationResult)
        {
        }

        public Paginator<ProductDto> GetListOfProducts(string query = null)
        {
            var request = new RestRequest("products", Method.GET);
            if (!string.IsNullOrWhiteSpace(query))
                request.AddQueryParameter("query", query);

            return new Paginator<ProductDto>(this, request);
        }

        public async Task<ProductDto> GetOneProduct(int productId)
        {
            if (productId < 1)
                throw new Exception("Product ID must be greater than 0.");
            var request = new RestRequest($"products/{productId}", Method.GET);

            return await CallBizzaro<ProductDto>(request);
        }

        public async Task<ProductDto> PostNewProduct(ProductDto productDto)
        {
            var request = new RestRequest("products", Method.POST);

            return await CallBizzaro<ProductDto>(request, productDto);
        }

        public async Task<StatusOnlyDto> UpdateProduct(int productId, ProductDto updatedProduct)
        {
            var request = new RestRequest("products/{id}", Method.PATCH);
            request.AddUrlSegment("id", productId.ToString());

            var status = await CallBizzaro(request, updatedProduct);

            return status;
        }
    }
}