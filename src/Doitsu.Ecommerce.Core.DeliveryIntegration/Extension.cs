using Doitsu.Ecommerce.Core.DeliveryIntegration.Configuration;
using Doitsu.Ecommerce.Core.DeliveryIntegration.GHTK;
using Doitsu.Ecommerce.Core.DeliveryIntegration.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Doitsu.Ecommerce.Core.DeliveryIntegration
{
    public static class Extension
    {
        public static void ConfigDeliveryIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DeliveryIntegrationSetting>(o => configuration.GetSection("DeliveryIntegration").Bind(o));
            services.Configure<GhtkPartnerConfiguration>(o => configuration.GetSection("DeliveryIntegration:Ghtk").Bind(o));
            services.AddScoped<IDeliveryIntegrator, DeliveryIntegrator>();
            services.AddScoped<IGhtkService, GhtkService>();
        }
    }
}