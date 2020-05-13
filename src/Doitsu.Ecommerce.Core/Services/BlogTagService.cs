using System.Linq;
using System.Threading.Tasks;
using Doitsu.Service.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.Data;
using System.Collections.Immutable;
using System.Collections.Generic;
using AutoMapper;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IBlogTagService : IBaseService<BlogTags>
    {
        Task DeleteAllByBlogIdAsync(int blogId);
        Task AddBlogTagFromTagTitles(int blogId, List<int> tagIds);
    }

    public class BlogTagService : BaseService<BlogTags>, IBlogTagService
    {
        public BlogTagService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<BlogTags, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }

        public async Task AddBlogTagFromTagTitles(int blogId, List<int> tagIds)
        {
            var blogTags = tagIds.Select(x => new BlogTags() {
                BlogId = blogId,
                TagId = x
            });
            await this.CreateAsync(blogTags);
        }

        public async Task DeleteAllByBlogIdAsync(int blogId)
        {
            var allBlogTags = await this
                .Get(x => x.BlogId == blogId)
                .Select(x => x.Id)
                .ToListAsync();

            await this.DeleteAsync<int>(allBlogTags);
        }
    }
}
