﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using Doitsu.Ecommerce.Core.Abstraction;
using System.Linq;

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
        Task<ImmutableList<BlogOverviewViewModel>> GetRandomBlogOverviewAsync(int take, int timeCache = 30);
        Task<ImmutableList<BlogDetailViewModel>> GetPromotionBlogDetailsAsync(int take, int cachingMinutes = 30);
        Task<ImmutableList<CategoryWithInverseParentViewModel>> GetInverseCategoryMenuAsync(int timeCache = 30);
        Task<ImmutableList<CategoryWithProductOverviewViewModel>> GetProductsFromExistCategoryAsync(string parentSlug = "", int limit = 0, int timeCache = 30);
        Task<ImmutableList<TagViewModel>> GetTopTagsByBlogCategorySlugAsync(string blogCategorySlug, int top, int cachingTime = 15);
        Task<ImmutableList<BlogOverviewViewModel>> GetTopBlogPostByCategorySlugAsync(string blogCategorySlug, int top, int cachingTime = 15);
        Task<ImmutableList<BlogOverviewViewModel>> GetTopRandomBlogPostByCategorySlugAsync(int top, int cachingTime = 15);
    }


    public class MemCacheService : IMemCacheService
    {
        private readonly ICategoryService categoryService;
        private readonly IProductService productService;
        private readonly IMemoryCache memoryCache;
        private readonly IBrandService brandService;
        private readonly IBlogService blogService;
        private readonly ISliderService sliderService;
        private readonly ICatalogueService catalogueService;
        private readonly ITagService tagService;
        private readonly ILogger<MemCacheService> logger;
        private readonly IMapper mapper;

        public MemCacheService(ICategoryService categoryService,
                               IProductService productService,
                               IMemoryCache memoryCache,
                               IBrandService brandService,
                               IBlogService blogService,
                               ISliderService sliderService,
                               ICatalogueService catalogueService,
                               ILogger<MemCacheService> logger,
                               IMapper mapper,
                               ITagService tagService)
        {
            this.categoryService = categoryService;
            this.productService = productService;
            this.memoryCache = memoryCache;
            this.brandService = brandService;
            this.blogService = blogService;
            this.sliderService = sliderService;
            this.catalogueService = catalogueService;
            this.logger = logger;
            this.mapper = mapper;
            this.tagService = tagService;
        }

        public async Task<BrandViewModel> GetBrandInformationAsync(int brandId, int timeCache = 30)
        {
            try
            {
                if (!memoryCache.TryGetValue(Constants.CacheKey.BRAND_INFORMATION, out BrandViewModel brand))
                {
                    var brandE = await brandService.FindByKeysAsync(brandId);
                    if (brandE == null) brandE = new Brand();
                    brand = mapper.Map<BrandViewModel>(brandE);
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

        public async Task<ImmutableList<BlogOverviewViewModel>> GetRandomBlogOverviewAsync(int take, int cachingMinutes = 30)
        {
            try
            {
                var key = $"{Constants.CacheKey.RANDOM_BLOGS}";
                if (!memoryCache.TryGetValue(key, out ImmutableList<BlogOverviewViewModel> randomBlogs))
                {
                    randomBlogs = await blogService.GetRandomOverviewAsync(take);
                    memoryCache.Set(key, randomBlogs, TimeSpan.FromMinutes(cachingMinutes));
                }
                return randomBlogs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(GetRandomBlogOverviewAsync)} have a exception");
                return ImmutableList<BlogOverviewViewModel>.Empty;
            }
        }

        public async Task<ImmutableList<CatalogueViewModel>> GetCataloguesAsync(int timeCache = 60)
        {
            try
            {
                if (!memoryCache.TryGetValue(Constants.CacheKey.CATALOGUES, out ImmutableList<CatalogueViewModel> catalogues))
                {
                    catalogues = (await catalogueService.GetAll().ProjectTo<CatalogueViewModel>(mapper.ConfigurationProvider).ToListAsync()).ToImmutableList();
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
                    sliders = (await sliderService.GetAll().ProjectTo<SliderViewModel>(mapper.ConfigurationProvider).ToListAsync()).ToImmutableList();
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

        public async Task<ImmutableList<BlogDetailViewModel>> GetPromotionBlogDetailsAsync(int take, int cachingMinutes = 30)
        {
            try
            {
                var key = $"{Constants.CacheKey.PROMOTION_BLOGS}";

                if (!memoryCache.TryGetValue(key, out ImmutableList<BlogDetailViewModel> promotionBlogs))
                {
                    promotionBlogs = (await blogService.GetPromotionBlogDetails(0, take)).Result;
                    memoryCache.Set(key, promotionBlogs, TimeSpan.FromMinutes(cachingMinutes));
                }

                return promotionBlogs;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(GetPromotionBlogDetailsAsync)} have a exception");
                return ImmutableList<BlogDetailViewModel>.Empty;
            }
        }

        public async Task<ImmutableList<CategoryWithInverseParentViewModel>> GetInverseCategoryMenuAsync(int timeCache = 30)
        {
            try
            {
                if (!memoryCache.TryGetValue(Constants.CacheKey.INVERSE_CATEGORY, out IEnumerable<CategoryWithInverseParentViewModel> inverseMenu))
                {
                    inverseMenu = await categoryService.GetAllParentCategoryWithInverseCategory();
                    memoryCache.Set(Constants.CacheKey.INVERSE_CATEGORY, inverseMenu, TimeSpan.FromMinutes(timeCache));
                }
                return inverseMenu.ToImmutableList();
            }
            catch
            {
                return ImmutableList<CategoryWithInverseParentViewModel>.Empty;
            }
        }

        public async Task<ImmutableList<TagViewModel>> GetTopBlogTagsAsync(int limit, int cachingMinutes)
        {
            var key = $"{Constants.CacheKey.TOP_TAGS}_{limit}";
            if (!memoryCache.TryGetValue(key, out ImmutableList<TagViewModel> topTags))
            {
                var tags = await tagService.GetTopBlogTagsAsync(limit);
                memoryCache.Set(key, tags.ToImmutableList(), TimeSpan.FromMinutes(cachingMinutes));
            }
            return topTags;
        }

        public async Task<ImmutableList<TagViewModel>> GetTopTagsByBlogCategorySlugAsync(string blogCategorySlug, int top, int cachingTime = 15)
        {
            var key = $"{Constants.CacheKey.TOP_TAGS}_{top}";
            if (!memoryCache.TryGetValue(key, out ImmutableList<TagViewModel> topTags))
            {
                var tags = this.tagService.GetAll()
                    .Where(x => x.BlogTags
                        .Where(bt => bt.Blog.BlogCategory.Slug == blogCategorySlug).Any())
                    .OrderByDescending(x => x.BlogTags.Count).Skip(0).Take(top);

                var result = await tags.ProjectTo<TagViewModel>(this.tagService.Mapper.ConfigurationProvider).ToListAsync();
                topTags = result.ToImmutableList();
                memoryCache.Set(key, topTags, TimeSpan.FromMinutes(cachingTime));
            }
            return topTags;
        }

        public async Task<ImmutableList<CategoryWithProductOverviewViewModel>> GetProductsFromExistCategoryAsync(string parentSlug = "", int limit = 0, int timeCache = 30)
        {
            var key = $"{Constants.CacheKey.PRODUCTS_FROM_EXIST_CATEGORIES}_{limit}";
            if (!memoryCache.TryGetValue(key, out ImmutableList<CategoryWithProductOverviewViewModel> listCategoryWithProducts))
            {
                listCategoryWithProducts = await this.categoryService.GetAllCategoriesWithProductAsync(parentSlug, limit);
                memoryCache.Set(key, listCategoryWithProducts, TimeSpan.FromMinutes(timeCache));
            }
            return listCategoryWithProducts;
        }

        public async Task<ImmutableList<BlogOverviewViewModel>> GetTopBlogPostByCategorySlugAsync(string blogCategorySlug, int top, int cachingTime = 15)
        {
            var key = $"{Constants.CacheKey.TOP_BLOG_POSTS}_{top}";
            if (!memoryCache.TryGetValue(key, out ImmutableList<BlogOverviewViewModel> blogPosts))
            {
                var list = await this.blogService.Get(bp => bp.BlogCategory.Slug == blogCategorySlug)
                    .OrderByDescending(x => x.PublishedTime)
                    .Skip(0)
                    .Take(top)
                    .ProjectTo<BlogOverviewViewModel>(this.blogService.Mapper.ConfigurationProvider).ToListAsync();

                blogPosts = list.ToImmutableList();
                memoryCache.Set(key, blogPosts, TimeSpan.FromMinutes(cachingTime));
            }
            return blogPosts;
        }

        public async Task<ImmutableList<BlogOverviewViewModel>> GetTopRandomBlogPostByCategorySlugAsync(int top, int cachingTime = 15)
        {
            var key = $"{Constants.CacheKey.TOP_RANDOM_BLOG_POSTS}_{top}";
            if (!memoryCache.TryGetValue(key, out ImmutableList<BlogOverviewViewModel> blogPosts))
            {
                var list = await this.blogService.GetRandomOverviewAsync(top);
                blogPosts = list.ToImmutableList();
                memoryCache.Set(key, blogPosts, TimeSpan.FromMinutes(cachingTime));
            }
            return blogPosts;
        }
    }
}