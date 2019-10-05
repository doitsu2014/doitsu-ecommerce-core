using System;

using Doitsu.Ecommerce.Core.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Doitsu.Ecommerce.Core
{
    public static class EcommerceServiceConfig
    {
        public static IServiceCollection AddFurnitureServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IMemCacheService), typeof(MemCacheService));
            services.AddScoped(typeof(ICategoryService), typeof(CategoryService));
            services.AddScoped(typeof(IProductService), typeof(ProductService));
            services.AddScoped(typeof(IBrandService), typeof(BrandService));
            services.AddScoped(typeof(IOrderService), typeof(OrderService));
            services.AddScoped(typeof(IOrderItemService), typeof(OrderItemService));
            services.AddScoped(typeof(IBlogService), typeof(BlogService));
            services.AddScoped(typeof(IBlogTagService), typeof(BlogTagService));
            services.AddScoped(typeof(ITagService), typeof(TagService));
            services.AddScoped(typeof(IMarketingCustomerService), typeof(MarketingCustomerService));
            services.AddScoped(typeof(ICustomerFeedbackService), typeof(CustomerFeedbackService));
            services.AddScoped(typeof(IBlogCategoryService), typeof(BlogCategoryService));
            services.AddScoped(typeof(IEmailService), typeof(EmailService));
            services.AddScoped(typeof(IAspNetUserRoleService), typeof(AspNetUserRoleService));
            services.AddScoped(typeof(IAspNetUserService), typeof(AspNetUserService));
            services.AddHttpContextAccessor();
            return services;
        }

    }
}