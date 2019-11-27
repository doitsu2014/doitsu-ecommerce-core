using AutoMapper;
using Doitsu.Ecommerce.Core.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Data.Identities;

namespace Doitsu.Ecommerce.Core
{
    public static class EcommerceMapperConfig
    {
        public static IServiceCollection AddFurnitureAutoMapper(this IServiceCollection services)
        {
            var autoMapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullDestinationValues = false;

                #region User Mapper
                cfg.CreateMap<EcommerceIdentityUser, UserInforViewModel>();
                cfg.CreateMap<RegisterViewModel, EcommerceIdentityUser>()
                    .AfterMap((src, desc) =>
                    {
                        desc.UserName = src.PhoneNumber;
                    });
                cfg.CreateMap<EcommerceIdentityUser, EcommerceIdentityUserViewModel>();
                cfg.CreateMap<EcommerceIdentityUserViewModel, EcommerceIdentityUser>();
                cfg.CreateMap<EcommerceIdentityRole, EcommerceIdentityRoleViewModel>();
                #endregion

                cfg.CreateMap<Categories, CategoryMenuViewModel>();
                cfg.CreateMap<Categories, CategoryViewModel>();
                cfg.CreateMap<Categories, CategoryWithProductOverviewViewModel>();
                cfg.CreateMap<Categories, CategoryWithParentViewModel>().MaxDepth(3);
                cfg.CreateMap<Categories, CategoryWithInverseParentViewModel>().MaxDepth(5);

                cfg.CreateMap<Products, ProductViewModel>();
                cfg.CreateMap<Products, ProductOverviewViewModel>()
                    .ForMember(dest => dest.CategorySlug, opt => opt.MapFrom(x => x.Cate.Slug))
                    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(x => x.Cate.Name));

                cfg.CreateMap<Products, ProductDetailViewModel>()
                    .ForMember(dest => dest.CategorySlug, opt => opt.MapFrom(x => x.Cate.Slug))
                    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(x => x.Cate.Name));

                cfg.CreateMap<ProductDetailViewModel, Products>();

                cfg.CreateMap<BrandViewModel, Brand>();
                cfg.CreateMap<Brand, BrandViewModel>();

                cfg.CreateMap<OrderViewModel, Orders>();
                cfg.CreateMap<Orders, OrderViewModel>();
                cfg.CreateMap<Orders, OrderDetailViewModel>();

                cfg.CreateMap<OrderItems, OrderItemViewModel>().ReverseMap();
                cfg.CreateMap<OrderItemViewModel, OrderItems>();

                cfg.CreateMap<Blogs, BlogDetailViewModel>();
                cfg.CreateMap<Blogs, BlogOverviewViewModel>();

                cfg.CreateMap<BlogDetailViewModel, Blogs>()
                    .ForMember(d => d.BlogCategory, opt => opt.Ignore())
                    .ForMember(d => d.PublisherId, opt => opt.Ignore())
                    .ForMember(d => d.Publisher, opt => opt.Ignore())
                    .ForMember(d => d.CreaterId, opt => opt.Ignore())
                    .ForMember(d => d.Creater, opt => opt.Ignore())
                    .ForMember(d => d.PublishedTime, opt => opt.Ignore());

                cfg.CreateMap<BlogTags, BlogTagViewModel>();
                cfg.CreateMap<BlogTagViewModel, BlogTags>()
                    .ForMember(d => d.Tag, opt => opt.MapFrom(src => new Tag()
                    {
                        Title = src.TagTitle,
                        Slug = src.TagSlug
                    }))
                    .ForMember(d => d.Blog, opt => opt.Ignore());
                cfg.CreateMap<Tag, TagViewModel>();

                cfg.CreateMap<BlogCategories, BlogCategoryViewModel>();
                cfg.CreateMap<BlogCategoryViewModel, BlogCategories>();

                cfg.CreateMap<MarketingCustomers, MarketingCustomerViewModel>();
                cfg.CreateMap<MarketingCustomers, MarketingCustomerMoreInformationViewModel>();
                cfg.CreateMap<MarketingCustomerViewModel, MarketingCustomers>();

                cfg.CreateMap<CustomerFeedbacks, CustomerFeedbackViewModel>();
                cfg.CreateMap<CustomerFeedbacks, CustomerFeedbackOverviewViewModel>();
                cfg.CreateMap<CustomerFeedbackViewModel, CustomerFeedbacks>();

                cfg.CreateMap<SliderViewModel, Sliders>();
                cfg.CreateMap<Sliders, SliderViewModel>();

                cfg.CreateMap<CatalogueViewModel, Catalogues>();
                cfg.CreateMap<Catalogues, CatalogueViewModel>();
            });

            IMapper mapper = autoMapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }
    }
}
