using System;
using Doitsu.Ecommerce.DeliveryIntegration.Configuration;
using Doitsu.Ecommerce.DeliveryIntegration.GHTK;
using Doitsu.Ecommerce.DeliveryIntegration.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Doitsu.Ecommerce.DeliveryIntegration
{
    public static class Extension
    {
        public static void ConfigDevlieryIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GHTKPartnerConfiguration>(o => configuration.GetSection("DeliveryIntegration:GHTK").Bind(o));
            services.AddScoped<IDeliveryIntegrator, DeliveryIntegrator>();
            services.AddScoped<IGHTKService, GHTKService>();
        }
    }
}