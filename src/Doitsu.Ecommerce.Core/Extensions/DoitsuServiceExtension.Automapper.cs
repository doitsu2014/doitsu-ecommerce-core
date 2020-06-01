using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.DeliveryIntegration;
using Doitsu.Ecommerce.Core.SEO;
using Microsoft.Extensions.DependencyInjection;

namespace Doitsu.Ecommerce.Core.Extensions
{
    public static partial class DoitsuServiceExtension
    {
        internal static IServiceCollection AddDoitsuEcommerceCoreAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddCollectionMappers(),
                typeof(DoitsuEcommerceCoreMapperProfile).Assembly,
                typeof(DoitsuEcommerceCoreDeliveryIntegrationMapperProfile).Assembly,
                typeof(DoitsuEcommerceCoreSEOMapperProfile).Assembly);
            return services;
        }
    }
}