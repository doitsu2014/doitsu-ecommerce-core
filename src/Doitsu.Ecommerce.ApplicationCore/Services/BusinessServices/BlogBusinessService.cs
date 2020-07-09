using System.Linq;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices;
using System;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Specifications.TagSepcifications;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Specifications;
using Doitsu.Ecommerce.ApplicationCore.Specifications.BlogTagSpecifications;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;

namespace Doitsu.Ecommerce.ApplicationCore.Services.BusinessServices
{
    public class BlogBusinessService : IBlogBusinessService
    {
        private readonly ILogger<BlogBusinessService> logger;
        private readonly IBaseEcommerceRepository<Blogs> blogRepository;
        private readonly IBaseEcommerceRepository<BlogTags> blogTagRepository;
        private readonly IBaseEcommerceRepository<Tag> tagRepository;
        private readonly IEcommerceDatabaseManager databaseManager;

        public BlogBusinessService(IBaseEcommerceRepository<Blogs> blogRepository,
                                   IBaseEcommerceRepository<BlogTags> blogTagRepository,
                                   IBaseEcommerceRepository<Tag> tagRepository,
                                   IEcommerceDatabaseManager databaseManager,
                                   ILogger<BlogBusinessService> logger)
        {
            this.blogRepository = blogRepository;
            this.blogTagRepository = blogTagRepository;
            this.tagRepository = tagRepository;
            this.databaseManager = databaseManager;
            this.logger = logger;
        }

        private async Task<List<BlogTags>> MakeListBlogTagsAsync(int blogId, Tag[] listTags)
        {
            logger.LogDebug("{functionName}: {agr1} - {agr2}", nameof(MakeListBlogTagsAsync), blogId, listTags);
            var listExistTags = await tagRepository.ListAsync(new TagFilterSpecification(listTags.Select(t => t.Slug).ToArray()));
            var listExistTagSlugs = listExistTags.Select(t => t.Slug).ToArray();
            var listNotExistTags = listTags.Where(t => !listExistTagSlugs.Contains(t.Slug)).ToList();

            var listBlogTags = new List<BlogTags>(); 
            listBlogTags.AddRange(listExistTags.Select(t => new BlogTags() { BlogId = blogId, TagId = t.Id }));
            listBlogTags.AddRange(listNotExistTags.Select(t => new BlogTags() 
            {
                BlogId = blogId,
                Tag = new Tag() 
                {
                    Title = t.Title,
                    Slug = t.Slug
                }
            }));
            return listBlogTags;
        }

        public async Task<Blogs> CreateBlogWithTagsAsync(Blogs blogEntity, Tag[] listTags)
        {
            logger.LogDebug("{functionName}: {agr1} - {agr2}", nameof(CreateBlogWithTagsAsync), blogEntity, listTags);
            blogEntity.BlogTags = await MakeListBlogTagsAsync(blogEntity.Id, listTags);
            return await this.blogRepository.AddAsync(blogEntity); 
        }

        public async Task UpdateBlogWithTagsAsync(Blogs blogEntity, Tag[] listTags)
        {
            logger.LogDebug("{functionName}: {agr1} - {agr2}", nameof(UpdateBlogWithTagsAsync), blogEntity, listTags);
            using(var transaction = await this.databaseManager.GetDatabaseContextTransactionAsync())
            {
                var listCurrentBlogTags = await blogTagRepository.ListAsync(new BlogTagFilterSpecification(blogEntity.Id));
                await this.blogTagRepository.DeleteRangeAsync(listCurrentBlogTags.ToArray());
                blogEntity.BlogTags = await MakeListBlogTagsAsync(blogEntity.Id, listTags);
                await this.blogRepository.UpdateAsync(blogEntity); 
                await transaction.CommitAsync();
            }
        }
    }
}
