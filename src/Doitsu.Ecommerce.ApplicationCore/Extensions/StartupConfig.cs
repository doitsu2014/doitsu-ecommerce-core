
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Doitsu.Ecommerce.ApplicationCore.Services.IdentityManagers;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Doitsu.Ecommerce.ApplicationCore.AuthorizeBuilder;
using AutoMapper;
using Doitsu.Ecommerce.ApplicationCore.AutoMapperProfiles;
using AutoMapper.EquivalencyExpression;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.ViewModelServices;
using Doitsu.Ecommerce.ApplicationCore.Services.ViewModelServices;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices;
using Doitsu.Ecommerce.ApplicationCore.Services.BusinessServices;

namespace Doitsu.Ecommerce.ApplicationCore.Extensions
{
    public static partial class StartupConfig
    {
        public static IServiceCollection AddDoitsuEcommerceCore(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            
            #region Identity Managers
            services.AddScoped(typeof(EcommerceIdentityUserManager<EcommerceIdentityUser>));
            services.AddScoped(typeof(EcommerceRoleIntManager<EcommerceIdentityRole>));
            services.AddScoped(typeof(EcommerceUserSignInManager<EcommerceIdentityUser>));
            services.AddScoped(typeof(IUserClaimsPrincipalFactory<EcommerceIdentityUser>), typeof(DoitsuCookieIdenittyCustomClaimsFactory<EcommerceIdentityUser>));
            #endregion

            #region Config service
            //services.AddDoitsuEcommerceCoreServices();
            services.AddScoped(typeof(IBlogBusinessService), typeof(BlogBusinessService));
            services.AddScoped(typeof(IProductBusinessService), typeof(ProductBusinessService));
            services.AddScoped(typeof(IBaseEcommerceViewModelService<>), typeof(BaseEcommerceViewModelService<>));
            #endregion

            #region Mapper Config
            services.AddAutoMapper(cfg => cfg.AddCollectionMappers(), typeof(DoitsuEcommerceCoreMapperProfile).Assembly);
            #endregion

            #region Cache Config
            services.AddMemoryCache();
            #endregion

            return services;
        }
    }
}
