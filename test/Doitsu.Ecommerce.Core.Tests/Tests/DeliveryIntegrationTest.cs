﻿using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.DeliveryIntegration;
using Doitsu.Ecommerce.Core.DeliveryIntegration.Common;
using Doitsu.Ecommerce.Core.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Doitsu.Ecommerce.Core.Tests
{
    [Collection("EcommerceCoreCollection")]
    public class DeliveryIntegrationTest : BaseServiceTest<EcommerceCoreFixture>
    {
        private readonly string _poolKey = nameof(DeliveryIntegrationTest);

        public DeliveryIntegrationTest(EcommerceCoreFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
        {
        }

        [Fact]
        private async Task Test_CalculateFees()
        {
            using (var webhost = WebHostBuilderHelper.BuilderWebhostWithInmemoryDb(_poolKey).Build())
            {
                var scopeFactory = webhost.Services.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    // Add Products
                    var deliveryIntegrator = scope.ServiceProvider.GetService<IDeliveryIntegrator>();
                    (await deliveryIntegrator.CalculateShipFeeAsync(DeliverEnum.Ghtk, new CalculateDeliveryFeesRequestModel()
                    {
                        PickProvince = "Hồ Chí Minh",
                        PickDistrict = "11",
                        PickWard = "5",
                        PickAddress = "424 Lô D, đường Tống Văn Trân",
                        Province = "Hồ Chí Minh",
                        District = "3",
                        Ward = "6",
                        Address = "147-149 Võ Văn Tần",
                    }))
                    .Match(res => Assert.True(res > 15000), error => Assert.True(false));

                }
              
            }
        }
    }
}