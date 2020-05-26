using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Doitsu.Ecommerce.Core.SEO
{
    public class EcommerceSEOMappingProfile : Profile
    {
        public EcommerceSEOMappingProfile()
        {
        }
    }

    public static class EcommerceSEOMapperConfig
    {
        public static IServiceCollection AddEcommerceSEOAutoMapper(this IServiceCollection services)
        {
            return services;
        }
    }
}
