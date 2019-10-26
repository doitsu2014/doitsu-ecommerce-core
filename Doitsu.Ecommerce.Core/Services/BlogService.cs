﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Ecommerce.Core.ViewModels;
using Doitsu.Service.Core;
using Doitsu.Service.Core.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Doitsu.Ecommerce.Core.Services
{
    public interface IBlogService : IBaseService<Blogs>
    {
        Task<DoitsuPaginatedList<BlogOverviewViewModel>> GetAllDetailBlogsByCategoryWithPaging(string blogCategorySlug, int page = 0, int limit = 4);
        Task<BlogDetailViewModel> GetBlogDetailBySlugAsync(string slug);
        Task<ImmutableList<BlogOverviewViewModel>> GetRandomOverviewAsync(int take = 5);
        Task<int> UpdateWithConstraintAsync(BlogDetailViewModel data, EcommerceIdentityUser publisher);
        Task<int> CreateWithConstraintAsync(BlogDetailViewModel data, EcommerceIdentityUser publisher, EcommerceIdentityUser creater);
    }

    public class BlogService : BaseService<Blogs>, IBlogService
    {

        private readonly IBlogTagService blogTagService;

        public BlogService(EcommerceDbContext dbContext,
                           IMapper mapper,
                           ILogger<BaseService<Blogs, EcommerceDbContext>> logger,
                           IBlogTagService blogTagService) : base(dbContext, mapper, logger)
        {
            this.blogTagService = blogTagService;
        }

        public async Task<ImmutableList<BlogOverviewViewModel>> GetRandomOverviewAsync(int take)
        {
            var rand = new Random();
            var blogsQuery = this.GetAll();
            var listShuffleOverview =
                await blogsQuery
                .ProjectTo<BlogOverviewViewModel>(Mapper.ConfigurationProvider)
                .OrderByDescending(x => Guid.NewGuid())
                .Skip(0)
                .Take(take)
                .ToListAsync();
            return listShuffleOverview.ToImmutableList();
        }

        public async Task<DoitsuPaginatedList<BlogOverviewViewModel>> GetAllDetailBlogsByCategoryWithPaging(string blogCategorySlug, int page = 0, int limit = 4)
        {
            var blogsQuery = this
                .Get(x => x.BlogCategory.Slug == blogCategorySlug);

            var result = blogsQuery
                .ProjectTo<BlogOverviewViewModel>(Mapper.ConfigurationProvider)
                .OrderByDescending(x => x.PublishedTime);

            var pagingWrapper = await DoitsuPaginatedList<BlogOverviewViewModel>.CreateAsync(result, page, limit);

            return pagingWrapper;
        }

        public async Task<BlogDetailViewModel> GetBlogDetailBySlugAsync(string slug)
        {
            var blog = await this.FirstOrDefaultAsync<BlogDetailViewModel>(x => x.Slug == slug);
            return blog;
        }

        public async Task<int> UpdateWithConstraintAsync(BlogDetailViewModel data, EcommerceIdentityUser publisher)
        {
            using (var transaction = await this.CreateTransactionAsync())
            {
                // remove all blog tags
                await blogTagService.DeleteAllByBlogIdAsync(data.Id);
                await CommitWithoutBeforeSavingAsync();

                var blogEntity = await FirstOrDefaultAsync(x => x.Id == data.Id);
                blogEntity = Mapper.Map(data, blogEntity);
                blogEntity.PublisherId = publisher.Id;
                await AddUiBlogTagToBlogEntity(blogEntity, data.BlogTags);

                this.Update(blogEntity);
                await CommitAsync();

                transaction.Commit();
                return blogEntity.Id;
            }
        }

        public async Task<int> CreateWithConstraintAsync(BlogDetailViewModel data, EcommerceIdentityUser publisher, EcommerceIdentityUser creater)
        {
            using (var transaction = await CreateTransactionAsync())
            {
                var blogEntity = Mapper.Map<Blogs>(data);
                blogEntity.PublisherId = publisher.Id;
                blogEntity.CreaterId = publisher.Id;
                blogEntity.PublishedTime = DateTime.Now;
                await AddUiBlogTagToBlogEntity(blogEntity, data.BlogTags);

                await CreateAsync(blogEntity);
                await CommitAsync();
                transaction.Commit();
                return blogEntity.Id;
            }
        }

        private async Task AddUiBlogTagToBlogEntity(Blogs blog, ICollection<BlogTagViewModel> blogTags)
        {
            blog.BlogTags = new List<BlogTags>();
            foreach (var bt in blogTags)
            {
                var existTag = await GetRepository<Tag>().AsNoTracking().FirstOrDefaultAsync(t => t.Slug == bt.TagSlug);
                if (existTag != null)
                {
                    blog.BlogTags.Add(new BlogTags()
                    {
                        BlogId = blog.Id,
                        TagId = existTag.Id
                    });
                }
                else
                {
                    blog.BlogTags.Add(new BlogTags()
                    {
                        BlogId = blog.Id,
                        Tag = new Tag()
                        {
                            Title = bt.TagTitle,
                            Slug = bt.TagSlug,
                        }
                    });
                }
            }
        }
    }
}