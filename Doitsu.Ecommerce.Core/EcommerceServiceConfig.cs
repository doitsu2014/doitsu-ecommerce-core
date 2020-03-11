using System;

using Doitsu.Ecommerce.Core.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Doitsu.Ecommerce.Core
{
    public static class EcommerceServiceConfig
    {
        public static IServiceCollection AddFurnitureServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(IMemCacheService), typeof(MemCacheService));
            services.AddTransient(typeof(ICategoryService), typeof(CategoryService));
            services.AddTransient(typeof(IProductService), typeof(ProductService));
            services.AddTransient(typeof(IBrandService), typeof(BrandService));
            services.AddTransient(typeof(IOrderService), typeof(OrderService));
            services.AddTransient(typeof(IOrderItemService), typeof(OrderItemService));
            services.AddTransient(typeof(IBlogService), typeof(BlogService));
            services.AddTransient(typeof(IBlogTagService), typeof(BlogTagService));
            services.AddTransient(typeof(ITagService), typeof(TagService));
            services.AddTransient(typeof(IMarketingCustomerService), typeof(MarketingCustomerService));
            services.AddTransient(typeof(ICustomerFeedbackService), typeof(CustomerFeedbackService));
            services.AddTransient(typeof(IBlogCategoryService), typeof(BlogCategoryService));
            services.AddTransient(typeof(IEmailService), typeof(EmailService));
            services.AddTransient(typeof(ISliderService), typeof(SliderService));
            services.AddTransient(typeof(ICatalogueService), typeof(CatalogueService));
            services.AddTransient(typeof(IPromotionDetailService), typeof(PromotionDetailService));
            services.AddTransient(typeof(IUserTransactionService), typeof(UserTransactionService));
            services.AddTransient(typeof(IProductVariantService), typeof(ProductVariantService));
            services.AddTransient(typeof(IProductOptionService), typeof(ProductOptionService));
            services.AddTransient(typeof(IUserService), typeof(UserService));
            services.AddHttpContextAccessor();
            return services;
        }

    }
}