using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Doitsu.Service.Core;
using Doitsu.Service.Core.Abstraction;
using Doitsu.Ecommerce.Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Data.Identities;

namespace Doitsu.Ecommerce.Core.Services {
    public interface IBlogService : IBaseService<Blogs> {
        Task<DoitsuPaginatedList<BlogOverviewViewModel>> GetAllDetailBlogsByCategoryWithPaging (string blogCategorySlug, int page = 0, int limit = 4);
        Task<BlogDetailViewModel> GetBlogDetailBySlugAsync (string slug);
        Task<ImmutableList<BlogOverviewViewModel>> GetRandomOverviewAsync (int take = 5, int cachingMinutes = 30);
        Task<int> UpdateWithConstraintAsync (BlogDetailViewModel data, EcommerceIdentityUser publisher);
        Task<int> CreateWithConstraintAsync (BlogDetailViewModel data, EcommerceIdentityUser publisher, EcommerceIdentityUser creater);
    }

    public class BlogService : BaseService<Blogs>, IBlogService {
        private readonly IMemoryCache memoryCache;
        public BlogService (IUnitOfWork unitOfWork, ILogger<BaseService<Blogs>> logger, IMemoryCache memoryCache) : base (unitOfWork, logger) {
            this.memoryCache = memoryCache;
        }

        public async Task<ImmutableList<BlogOverviewViewModel>> GetRandomOverviewAsync (int take, int cachingMinutes = 30) {
            var key = $"{Constants.CacheKey.RANDOM_BLOGS}";
            if (!memoryCache.TryGetValue (key, out ImmutableList<BlogOverviewViewModel> randomBlogs)) {
                var rand = new Random ();
                var blogsQuery = this.GetAll();
                var listShuffleOverview =
                    await blogsQuery
                    .ProjectTo<BlogOverviewViewModel> (this.UnitOfWork.Mapper.ConfigurationProvider)
                    .OrderByDescending (x => Guid.NewGuid ())
                    .Skip (0)
                    .Take (take)
                    .ToListAsync ();

                randomBlogs = listShuffleOverview.ToImmutableList ();
                memoryCache.Set (key, randomBlogs, TimeSpan.FromMinutes (cachingMinutes));
            }
            return randomBlogs;
        }

        public async Task<DoitsuPaginatedList<BlogOverviewViewModel>> GetAllDetailBlogsByCategoryWithPaging (string blogCategorySlug, int page = 0, int limit = 4) {
            var blogsQuery = this
                .Get(x => x.BlogCategory.Slug == blogCategorySlug);

            var result = blogsQuery
                .ProjectTo<BlogOverviewViewModel> (this.UnitOfWork.Mapper.ConfigurationProvider)
                .OrderByDescending (x => x.PublishedTime);

            var pagingWrapper = await DoitsuPaginatedList<BlogOverviewViewModel>.CreateAsync (result, page, limit);

            return pagingWrapper;
        }

        public async Task<BlogDetailViewModel> GetBlogDetailBySlugAsync (string slug) {
            var blog = await this.FirstOrDefaultAsync<BlogDetailViewModel> (x => x.Slug == slug);
            return blog;
        }

        public async Task<int> UpdateWithConstraintAsync (BlogDetailViewModel data, EcommerceIdentityUser publisher) {
            using (var transaction = await this.UnitOfWork.CreateTransactionAsync ()) {
                try {
                    var existBlog = await FirstOrDefaultAsync (x => x.Id == data.Id);
                    existBlog = this.UnitOfWork.Mapper.Map (data, existBlog);
                    existBlog.PublisherId = publisher.Id;
                    // remove all blog tags
                    var blogTagService = this.UnitOfWork.GetService<IBlogTagService> ();
                    var tagService = this.UnitOfWork.GetService<ITagService> ();

                    await blogTagService.DeleteAllByBlogIdAsync (existBlog.Id);
                    await this.UnitOfWork.CommitAsync ();
                    // update tag id and blog id
                    existBlog.BlogTags = existBlog.BlogTags.Select (bt => {
                        bt.BlogId = existBlog.Id;
                        bt.Tag.Active = true;
                        bt.Active = true;
                        var existTag = tagService.FirstOrDefaultAsync(t => t.Slug == bt.Tag.Slug);
                        if (existTag != null) {
                            bt.TagId = existTag.Id;
                            bt.Tag = null;
                        }
                        return bt;
                    }).ToList ();

                    this.Update (existBlog);
                    await this.UnitOfWork.CommitAsync ();
                    transaction.Commit ();
                    return existBlog.Id;
                } catch (Exception ex) {
                    Logger.LogError (ex, "Update blog exception");
                    transaction.Dispose ();
                    throw (ex);
                }
            }
        }

        public async Task<int> CreateWithConstraintAsync (BlogDetailViewModel data, EcommerceIdentityUser publisher, EcommerceIdentityUser creater) {
            using (var transaction = await this.UnitOfWork.CreateTransactionAsync ()) {
                try {
                    var blogEntity = this.UnitOfWork.Mapper.Map<Blogs> (data);
                    blogEntity.PublisherId = publisher.Id;
                    blogEntity.CreaterId = publisher.Id;
                    blogEntity.PublishedTime = DateTime.Now;
                    // update tag id and blog id
                    var tagService = this.UnitOfWork.GetService<ITagService> ();
                    blogEntity.BlogTags = blogEntity.BlogTags.Select (bt => {
                        bt.BlogId = blogEntity.Id;
                        bt.Tag.Active = true;
                        bt.Active = true;

                        var existTag = tagService.FirstOrDefaultAsync(t => t.Slug == bt.Tag.Slug);
                        if (existTag != null) {
                            bt.TagId = existTag.Id;
                            bt.Tag = null;
                        }
                        return bt;
                    }).ToList ();

                    await this.CreateAsync (blogEntity);
                    await this.UnitOfWork.CommitAsync ();
                    transaction.Commit ();
                    return blogEntity.Id;
                } catch (Exception ex) {
                    Logger.LogError (ex, "CreateWithConstraintAsync exception:");
                    transaction.Dispose ();
                    throw (ex);
                }
            }
        }
    }
}