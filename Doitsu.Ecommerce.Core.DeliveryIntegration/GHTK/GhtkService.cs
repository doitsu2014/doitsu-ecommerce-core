using System;
using System.Net.Http;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.DeliveryIntegration.Common;
using Doitsu.Ecommerce.Core.DeliveryIntegration.Configuration;
using Doitsu.Ecommerce.Core.DeliveryIntegration.GHTK.Models.Response;
using Doitsu.Ecommerce.Core.DeliveryIntegration.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Optional;
using Optional.Async;

namespace Doitsu.Ecommerce.Core.DeliveryIntegration.GHTK
{
    public class GhtkService : IGhtkService
    {
        private GhtkPartnerConfiguration configuration;
        public GhtkService(IOptions<GhtkPartnerConfiguration> options)
        {
            configuration = options.Value;
        }

        public async Task<Option<dynamic, string>> CalculateFees(CalculateDeliveryFeesRequestModel requestModel)
        {
            using (HttpClient client = new HttpClient())
            {
                return (await requestModel
                    .SomeNotNull()
                    .WithException("Data request is null or empty")
                    .Filter(d => !d.PickProvince.IsNullOrEmpty(), "Pick province is empty, please provide it.")
                    .Filter(d => !d.PickDistrict.IsNullOrEmpty(), "Pick district is empty, please provide it.")
                    .Filter(d => !d.Province.IsNullOrEmpty(), "Province is empty, please provide it.")
                    .Filter(d => !d.District.IsNullOrEmpty(), "District is empty, please provide it.")
                    .MapAsync(async d => {
                        client.DefaultRequestHeaders.Add("Token", configuration.ClientSecret);
                        var requestUrlWithParam = $"{configuration.CalculateFeesUrl}?pick_province={d.PickProvince}&pick_district={d.PickDistrict}&pick_ward={d.PickWard}&pick_address={d.PickAddress}&province={d.Province}&district={d.District}&ward={d.Ward}&address={d.Address}&weight={d.Weight}";
                        var response = await client.GetAsync(requestUrlWithParam);
                        return response; 
                    })
                    .FlatMapAsync(async httpResponse =>
                    {
                        var content = await httpResponse.Content.ReadAsStringAsync();
                        if (!httpResponse.IsSuccessStatusCode)
                        {
                            return Option.None<GHTKCalculationFeesResponse, string>(content);
                        }
                        else
                        {
                            var calculateFeeResponse = JsonConvert.DeserializeObject<GHTKCalculationFeesResponse>(content);
                            return Option.Some<GHTKCalculationFeesResponse, string>(calculateFeeResponse);
                        }
                    }))
                    .Map(d => d.Fee.Fee);
            }
        }
    }
}