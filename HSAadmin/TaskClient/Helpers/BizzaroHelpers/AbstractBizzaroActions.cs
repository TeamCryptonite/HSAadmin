using System;
using System.Threading.Tasks;
using System.Windows.Navigation;
using HsaServiceDtos;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using RestSharp;

// ReSharper disable once CheckNamespace

namespace TaskClient.Helpers.BizzaroHelpers
{
    public abstract class AbstractBizzaroActions
    {
        protected readonly string authToken;
        protected readonly RestClient client;

        protected AbstractBizzaroActions(AuthenticationResult authenticationResult)
        {
            string baseUrl = "https://bizzaro.azurewebsites.net/api";
            authToken = authenticationResult.Token;
            client = new RestClient(baseUrl);
        }

        private void AddHeadersToRequest(IRestRequest request, object bodyData = null)
        {
            request.AddParameter("Authorization", "Bearer " + authToken, ParameterType.HttpHeader);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");

            if (bodyData != null)
                request.AddParameter("application/json", bodyData, ParameterType.RequestBody);
        }

        public async Task<T> CallBizzaro<T>(IRestRequest request, object bodyData = null)
        {
            AddHeadersToRequest(request, bodyData);
            
            var response = await client.ExecuteTaskAsync<T>(request);

            if ((int)response.StatusCode < 200 || (int)response.StatusCode > 399)
                throw new Exception("Could not process HTTP call. " + response.StatusDescription + ". " +
                                    response.Content);

            return response.Data;
        }

       

        public async Task<StatusOnlyDto> CallBizzaro(IRestRequest request, object bodyData = null)
        {
            var statusReturn = new StatusOnlyDto();
            AddHeadersToRequest(request, bodyData);

            var response = await client.ExecuteTaskAsync(request);

            if ((int)response.StatusCode < 200 || (int)response.StatusCode > 399)
                statusReturn.StatusMessage = "Could not process HTTP call. " + response.StatusDescription;
            else
                statusReturn.StatusMessage = "Success";

            return statusReturn;
        }

        public async Task<JArray> CallBizzaroJArray(IRestRequest request, object bodyData = null)
        {
            AddHeadersToRequest(request, bodyData);

            var response = await client.ExecuteTaskAsync(request);

            if ((int)response.StatusCode < 200 || (int)response.StatusCode > 399)
                throw new Exception("Could not process HTTP call. " + response.StatusDescription + ". " +
                                    response.Content);

            return JArray.Parse(response.Content);

            //return statusReturn;
        }
    }
}