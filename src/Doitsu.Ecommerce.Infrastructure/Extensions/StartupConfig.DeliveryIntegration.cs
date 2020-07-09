using Doitsu.Ecommerce.ApplicationCore.Interfaces.DeliveryIntegration;
using Doitsu.Ecommerce.ApplicationCore.Models.DeliveryIntegration;
using Doitsu.Ecommerce.Infrastructure.Services.DeliveryIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Doitsu.Ecommerce.Infrastructure.Extensions
{
    public static partial class StartupConfig
    {
        public static void AddDeliveryIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DeliveryIntegrationSetting>(o => configuration.GetSection("DeliveryIntegration").Bind(o));
            services.Configure<GhtkPartnerConfiguration>(o => configuration.GetSection("DeliveryIntegration:Ghtk").Bind(o));
            services.AddScoped<IDeliveryIntegrator, DeliveryIntegrator>();
            services.AddScoped<IGhtkService, GhtkService>();
        }
    }
}
