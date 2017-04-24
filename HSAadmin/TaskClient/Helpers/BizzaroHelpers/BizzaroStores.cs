using System;
using System.Threading.Tasks;
using HsaServiceDtos;
using Microsoft.Identity.Client;
using RestSharp;

namespace TaskClient.Helpers.BizzaroHelpers
{
    public class BizzaroStores : AbstractBizzaroActions
    {
        public BizzaroStores(AuthenticationResult authenticationResult) : base(authenticationResult)
        {
        }

        public Paginator<StoreDto> GetListOfStores(string query = null, int? productId = null, int? radius = null
            , LocationDto userLocation = null)
        {
            var request = new RestRequest("stores", Method.GET);
            if (!string.IsNullOrWhiteSpace(query))
                request.AddQueryParameter("query", query);
            if (productId.HasValue)
                request.AddQueryParameter("productid", productId.ToString());
            if (userLocation != null)
            {
                request.AddQueryParameter("userLat", userLocation.Latitude.ToString());
                request.AddQueryParameter("userLong", userLocation.Longitude.ToString());
            }

            if (radius.HasValue && userLocation != null)
                request.AddQueryParameter("radius", radius.ToString());

            return new Paginator<StoreDto>(this, request);
        }

        public async Task<StoreDto> GetOneStore(int storeId)
        {
            if (storeId < 1)
                throw new Exception("Store ID must be greater than 0.");
            var request = new RestRequest($"stores/{storeId}", Method.GET);

            return await CallBizzaro<StoreDto>(request);
        }

        public async Task<StoreDto> PostNewStore(StoreDto storeDto)
        {
            var request = new RestRequest("stores", Method.POST);

            return await CallBizzaro<StoreDto>(request, storeDto);
        }


        public async Task<StatusOnlyDto> UpdateStore(int storeId, StoreDto updatedStore)
        {
            var request = new RestRequest("stores/{id}", Method.PATCH);
            request.AddUrlSegment("id", storeId.ToString());

            var status = await CallBizzaro(request, updatedStore);

            return status;
        }
    }
}