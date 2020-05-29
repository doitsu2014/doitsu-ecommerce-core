using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using AutoMapper.EquivalencyExpression;
using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.DeliveryIntegration;
using Doitsu.Ecommerce.Core.SEO;

namespace Doitsu.Ecommerce.Core
{
    public static class EcommerceMapperConfig
    {
        public static IServiceCollection AddDoitsuEcommerceCoreAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddCollectionMappers(), 
                typeof(DoitsuEcommerceCoreMapperProfile).Assembly, 
                typeof(DoitsuEcommerceCoreDeliveryIntegrationMapperProfile).Assembly, 
                typeof(DoitsuEcommerceCoreSEOMapperProfile).Assembly);
            return services;
        }
    }
}
