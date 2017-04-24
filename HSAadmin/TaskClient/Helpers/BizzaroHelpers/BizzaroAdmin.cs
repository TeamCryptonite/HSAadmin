using System;
using System.Threading.Tasks;
using HsaServiceDtos;
using HSAManager.Helpers.BizzaroHelpers;
using Microsoft.Identity.Client;
using RestSharp;

namespace TaskClient.Helpers.BizzaroHelpers
{
    public class BizzaroAdmin : AbstractBizzaroActions
    {
        public BizzaroAdmin(AuthenticationResult authenticationResult) : base(authenticationResult)
        {
        }

        public Paginator<UserDto> GetListOfUsers(string query = null)
        {
            var request = new RestRequest("admin/users", Method.GET);
            if (!string.IsNullOrWhiteSpace(query))
                request.AddQueryParameter("query", query);

            return new Paginator<UserDto>(this, request);
        }

        public async Task<StatusOnlyDto> UpdateUser(Guid userGuid, UserDto user)
        {
            var request = new RestRequest($"admin/users/{userGuid}", Method.PATCH);

            return await CallBizzaro(request, user);
        }
    }
}