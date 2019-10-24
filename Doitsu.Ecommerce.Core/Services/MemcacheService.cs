using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Doitsu.Service.Core;
using Doitsu.Ecommerce.Core.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
namespace Doitsu.Ecommerce.Core.Services
{
    public interface IMemCacheService
    {
        Task<ImmutableList<CategoryMenuViewModel>> GetProductCategoryMenuAsync(int timeCache = 30);
        Task<ImmutableList<CategoryViewModel>> GetCategoryWithParentAsync(string parentCateSlug, int timeCache = 30);
        Task<BrandViewModel> GetBrandInformationAsync(int brandId, int timeCache = 30);
        Task<ImmutableList<ProductOverviewViewModel>> GetRandomProductAsync(int take = 10, int timeCache = 30);
        Task<ImmutableList<CategoryViewModel>> GetBuildingCategoryAsync(int timeCache = 60 * 24 * 30);
        Task<ImmutableList<SliderViewModel>> GetSlidersAsync(int timeCache = 60);
        Task<ImmutableList<CatalogueViewModel>> GetCataloguesAsync(int timeCache = 60);
    }

    public class MemCacheService : IMemCacheService
    {
        private readonly ICategoryService categoryService;
        private readonly IProductService productService;
        private readonly IMemoryCache memoryCache;
        private readonly IBrandService brandService;
        private readonly ISliderService sliderService;
        private readonly ICatalogueService catalogueService;
        private readonly ILogger<MemCacheService> logger;
        private readonly IEcommerceUnitOfWork unitOfWork;
        public MemCacheService(
            IEcommerceUnitOfWork unitOfWork,
            IMemoryCache memoryCache,
            ILogger<MemCacheService> logger)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.categoryService = unitOfWork.GetService<ICategoryService>();
            this.brandService = unitOfWork.GetService<IBrandService>();
            this.productService = unitOfWork.GetService<IProductService>();
            this.sliderService = unitOfWork.GetService<ISliderService>();
            this.catalogueService = unitOfWork.GetService<ICatalogueService>();
            this.memoryCache = memoryCache;
        }
        public async Task<BrandViewModel> GetBrandInformationAsync(int brandId, int timeCache = 30)
        {
            try
            {
                if (!memoryCache.TryGetValue(Constants.CacheKey.BRAND_INFORMATION, out BrandViewModel brand))
                {
                    var brandE = await brandService.FindByKeysAsync(brandId);
                    brand = unitOfWork.Mapper.Map<BrandViewModel>(brandE);
                    memoryCache.Set(Constants.CacheKey.BRAND_INFORMATION, brand, TimeSpan.FromMinutes(timeCache));
                }
                return brand;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var brand = new BrandViewModel();
                return brand;
            }
        }

        public async Task<ImmutableList<CategoryMenuViewModel>> GetProductCategoryMenuAsync(int timeCache = 30)
        {
            try
            {
                if (!memoryCache.TryGetValue(Constants.CacheKey.MENU_ITEMS, out IEnumerable<CategoryMenuViewModel> inernalMenu))
                {
                    inernalMenu = await categoryService.GetProductCategoryForMenuAsync();
                    memoryCache.Set(Constants.CacheKey.MENU_ITEMS, inernalMenu, TimeSpan.FromMinutes(timeCache));
                }
                return inernalMenu.ToImmutableList();
            }
            catch
            {
                return ImmutableList<CategoryMenuViewModel>.Empty;
            }
        }

        public async Task<ImmutableList<CategoryViewModel>> GetBuildingCategoryAsync(int timeCache = 60 * 24 * 30)
        {
            try
            {
                if (!memoryCache.TryGetValue(Constants.CacheKey.BUILDING_CATEGORY_CHILDREN, out ImmutableList<CategoryViewModel> inernalMenu))
                {
                    inernalMenu = await categoryService.GetBuildingCategoryChildrenAsync();
                    memoryCache.Set(Constants.CacheKey.BUILDING_CATEGORY_CHILDREN, inernalMenu, TimeSpan.FromMinutes(timeCache));
                }
                return inernalMenu;
            }
            catch
            {
                return ImmutableList<CategoryViewModel>.Empty;
            }
        }

        public async Task<ImmutableList<CategoryViewModel>> GetCategoryWithParentAsync(string parentCateSlug, int timeCache = 30)
        {
            try
            {
                if (!memoryCache.TryGetValue($"{Constants.CacheKey.MENU_ITEMS}_{parentCateSlug}", out IEnumerable<CategoryViewModel> inernalMenu))
                {
                    inernalMenu = await categoryService.GetCategoryForParentCateSlugAsync(parentCateSlug);
                    memoryCache.Set($"{Constants.CacheKey.MENU_ITEMS}_{parentCateSlug}", inernalMenu, TimeSpan.FromMinutes(timeCache));
                }
                return inernalMenu.ToImmutableList();
            }
            catch
            {
                return ImmutableList<CategoryViewModel>.Empty;
            }
        }

        public async Task<ImmutableList<ProductOverviewViewModel>> GetRandomProductAsync(int take = 10, int timeCache = 15)
        {
            try
            {
                if (!memoryCache.TryGetValue(Constants.CacheKey.RANDOM_PRODUCTS, out ImmutableList<ProductOverviewViewModel> randomProducts))
                {
                    randomProducts = await productService.GetRandomProductAsync(take);
                    memoryCache.Set(Constants.CacheKey.RANDOM_PRODUCTS, randomProducts, TimeSpan.FromMinutes(timeCache));
                }
                return randomProducts;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Get Random Product Async have a exception");
                return ImmutableList<ProductOverviewViewModel>.Empty;
            }
        }

        public async Task<ImmutableList<CatalogueViewModel>> GetCataloguesAsync(int timeCache = 60)
        {
            try
            {
                if (!memoryCache.TryGetValue(Constants.CacheKey.CATALOGUES, out ImmutableList<CatalogueViewModel> catalogues))
                {
                    catalogues = (await catalogueService.GetAll().ProjectTo<CatalogueViewModel>(unitOfWork.Mapper.ConfigurationProvider).ToListAsync()).ToImmutableList();
                    memoryCache.Set(Constants.CacheKey.CATALOGUES, catalogues, TimeSpan.FromMinutes(timeCache));
                }
                return catalogues;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Get Catalogues Async have a exception");
                return ImmutableList<CatalogueViewModel>.Empty;
            }
        }

        public async Task<ImmutableList<SliderViewModel>> GetSlidersAsync(int timeCache = 60)
        {
            try
            {
                if (!memoryCache.TryGetValue(Constants.CacheKey.SLIDERS, out ImmutableList<SliderViewModel> sliders))
                {
                    sliders = (await sliderService.GetAll().ProjectTo<SliderViewModel>(unitOfWork.Mapper.ConfigurationProvider).ToListAsync()).ToImmutableList();
                    memoryCache.Set(Constants.CacheKey.SLIDERS, sliders, TimeSpan.FromMinutes(timeCache));
                }
                return sliders;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Get Sliders Async have a exception");
                return ImmutableList<SliderViewModel>.Empty;
            }
        }

    }
}