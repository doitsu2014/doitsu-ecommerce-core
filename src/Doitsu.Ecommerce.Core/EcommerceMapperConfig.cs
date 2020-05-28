using AutoMapper;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Identities;
using Doitsu.Ecommerce.Core.DeliveryIntegration.Common;
using AutoMapper.EquivalencyExpression;

namespace Doitsu.Ecommerce.Core
{
    public class EcommerceMappingProfile : Profile
    {
        public EcommerceMappingProfile()
        {
            // AllowNullDestinationValues = false;

            #region User Mapper
            CreateMap<EcommerceIdentityUser, UserInforViewModel>();
            CreateMap<RegisterViewModel, EcommerceIdentityUser>()
                .AfterMap((src, desc) =>
                {
                    desc.UserName = src.PhoneNumber;
                });
            CreateMap<EcommerceIdentityUser, EcommerceIdentityUserViewModel>();
            CreateMap<UserInforViewModel, UpdateDetailViewModel>();
            CreateMap<EcommerceIdentityUserViewModel, EcommerceIdentityUser>();
            CreateMap<EcommerceIdentityRole, EcommerceIdentityRoleViewModel>();
            #endregion
            #region Categories
            CreateMap<Categories, CategoryMenuViewModel>().MaxDepth(5);
            CreateMap<Categories, CategoryViewModel>();
            CreateMap<Categories, CategoryWithProductOverviewViewModel>();
            CreateMap<Categories, CategoryWithParentViewModel>();
            CreateMap<Categories, CategoryWithoutParentViewModel>();
            CreateMap<Categories, CategoryWithInverseParentViewModel>().MaxDepth(5);
            CreateMap<CategoryMenuViewModel, Categories>();
            CreateMap<CategoryViewModel, Categories>();
            CreateMap<CategoryWithProductOverviewViewModel, Categories>();
            CreateMap<CategoryWithParentViewModel, Categories>();
            CreateMap<CategoryWithoutParentViewModel, Categories>();
            CreateMap<CategoryWithInverseParentViewModel, Categories>().MaxDepth(5);
            CreateMap<BaseCategoryViewModel, Categories>();
            CreateMap<Categories, BaseCategoryViewModel>();
            #endregion
            #region Products
            CreateMap<Products, ProductViewModel>();
            CreateMap<Products, ProductOverviewViewModel>()
                .ForMember(dest => dest.CategorySlug, opt => opt.MapFrom(x => x.Cate.Slug))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(x => x.Cate.Name));

            CreateMap<ProductDetailViewModel, Products>();
            CreateMap<Products, ProductDetailViewModel>()
                .ForMember(dest => dest.CategorySlug, opt => opt.MapFrom(x => x.Cate.Slug))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(x => x.Cate.Name));

            CreateMap<CreateProductViewModel, Products>()
                .ForMember(dest => dest.Cate, opts => opts.MapFrom(src => src.Cate ?? null));
            CreateMap<UpdateProductViewModel, Products>();
            CreateMap<Products, CreateProductViewModel>();
            CreateMap<Products, UpdateProductViewModel>();
            CreateMap<ProductOptionValues, ProductOptionValueViewModel>();
            CreateMap<ProductOptions, ProductOptionViewModel>();
            CreateMap<ProductOptionValueViewModel, ProductOptionValues>();
            CreateMap<ProductOptionViewModel, ProductOptions>();

            CreateMap<ProductVariants, ProductVariantViewModel>();
            CreateMap<ProductVariants, ProductVariantDetailViewModel>();
            CreateMap<ProductVariantOptionValues, ProductVariantOptionValueViewModel>();
            CreateMap<ProductVariantViewModel, ProductVariants>();
            CreateMap<ProductVariantOptionValueViewModel, ProductVariantOptionValues>();
            #endregion
            #region Catalogues
            CreateMap<CatalogueViewModel, Catalogues>();
            CreateMap<Catalogues, CatalogueViewModel>();
            #endregion
            #region Brands
            CreateMap<BrandViewModel, Brand>();
            CreateMap<Brand, BrandViewModel>();
            #endregion
            #region Orders
            CreateMap<OrderViewModel, Orders>();
            CreateMap<CreateOrderWithOptionViewModel, Orders>();
            CreateMap<Orders, OrderViewModel>();
            CreateMap<Orders, OrderDetailViewModel>().MaxDepth(3);

            CreateMap<OrderItems, OrderItemViewModel>().ReverseMap();
            CreateMap<OrderItemViewModel, OrderItems>();
            CreateMap<CreateOrderItemWithOptionViewModel, OrderItems>()
                .ForMember(o => o.ProductVariant, opt => opt.Ignore())
                .ForMember(o => o.Product, opt => opt.Ignore());
            #endregion
            CreateMap<Blogs, BlogDetailViewModel>();
            CreateMap<Blogs, BlogOverviewViewModel>();
            CreateMap<BlogDetailViewModel, Blogs>()
                .ForMember(d => d.BlogCategory, opt => opt.Ignore())
                .ForMember(d => d.PublisherId, opt => opt.Ignore())
                .ForMember(d => d.Publisher, opt => opt.Ignore())
                .ForMember(d => d.CreaterId, opt => opt.Ignore())
                .ForMember(d => d.Creater, opt => opt.Ignore())
                .ForMember(d => d.PublishedTime, opt => opt.Ignore());

            CreateMap<BlogTags, BlogTagViewModel>();
            CreateMap<BlogTagViewModel, BlogTags>()
                .ForMember(d => d.Tag, opt => opt.MapFrom(src => new Tag()
                {
                    Title = src.TagTitle,
                    Slug = src.TagSlug
                }))
                .ForMember(d => d.Blog, opt => opt.Ignore());
            CreateMap<Tag, TagViewModel>();

            CreateMap<BlogCategories, BlogCategoryViewModel>();
            CreateMap<BlogCategoryViewModel, BlogCategories>();

            CreateMap<MarketingCustomers, MarketingCustomerViewModel>();
            CreateMap<MarketingCustomers, MarketingCustomerMoreInformationViewModel>();
            CreateMap<MarketingCustomerViewModel, MarketingCustomers>();

            CreateMap<CustomerFeedbacks, CustomerFeedbackViewModel>();
            CreateMap<CustomerFeedbacks, CustomerFeedbackOverviewViewModel>();
            CreateMap<CustomerFeedbackViewModel, CustomerFeedbacks>();

            CreateMap<SliderViewModel, Sliders>();
            CreateMap<Sliders, SliderViewModel>();

            CreateMap<PromotionDetail, PromotionDetailViewModel>();
            CreateMap<PromotionDetailViewModel, PromotionDetail>();

            CreateMap<UserTransactionViewModel, UserTransaction>();
            CreateMap<UserTransaction, UserTransactionViewModel>();

            CreateMap<WareHouse, WareHouseViewModel>();
            CreateMap<WareHouseViewModel, WareHouse>();

            CreateMap<CreateOrderWithOptionViewModel, CalculateDeliveryFeesRequestModel>()
                .ForMember(d => d.Address, opt => opt.MapFrom(src => src.DeliveryAddress))
                .ForMember(d => d.District, opt => opt.MapFrom(src => src.DeliveryDistrict))
                .ForMember(d => d.Province, opt => opt.MapFrom(src => src.DeliveryCity))
                .ForMember(d => d.Ward, opt => opt.MapFrom(src => src.DeliveryWard));

            CreateMap<DeliveryInformationViewModel, DeliveryInformation>();
            CreateMap<DeliveryInformation, DeliveryInformationViewModel>();

        }

    }

    public static class EcommerceMapperConfig
    {
        public static IServiceCollection AddFurnitureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddCollectionMappers(), typeof(EcommerceMappingProfile).Assembly);
            return services;
        }
    }
}
