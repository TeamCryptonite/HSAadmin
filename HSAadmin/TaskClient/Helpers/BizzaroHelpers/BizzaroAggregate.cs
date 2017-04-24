using System;
using System.Threading.Tasks;
using HsaServiceDtos;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using RestSharp;
using TaskClient.Helpers.BizzaroHelpers;

namespace HSAManager.Helpers.BizzaroHelpers
{
    public class BizzaroAggregate : AbstractBizzaroActions
    {
        public enum TimePeriod
        {
            YearMonth, Day, Month, Year
        }
        public BizzaroAggregate(AuthenticationResult authenticationResult) : base(authenticationResult)
        {
        }

        public async Task<string> GetSpendingOverTime(DateTime? startDateStr = null, DateTime? endDateStr = null, TimePeriod? timePeriod = null)
        {
            var request = new RestRequest("receiptaggregate/spendingovertime", Method.GET);
            if (startDateStr.HasValue)
                request.AddQueryParameter("startDateStr", startDateStr.Value.ToString());
            if(endDateStr.HasValue)
                request.AddQueryParameter("endDateStr", endDateStr.Value.ToString());
            if (timePeriod.HasValue)
                request.AddQueryParameter("timePeriod", timePeriod.ToString().ToLower());

            var json = await CallBizzaroJArray(request);
            return json.ToString();
        }

        
    }
}