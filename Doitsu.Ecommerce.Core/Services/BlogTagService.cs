using System.Linq;
using System.Threading.Tasks;
using Doitsu.Service.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.Data;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IBlogTagService : IBaseService<BlogTags>
    {
        Task DeleteAllByBlogIdAsync(int blogId);
        Task AddBlogTagFromTagTitles(int blogId, List<int> tagIds);
    }

    public class BlogTagService : BaseService<BlogTags>, IBlogTagService
    {
        public BlogTagService(IEcommerceUnitOfWork unitOfWork, ILogger<BaseService<BlogTags, EcommerceDbContext, IEcommerceUnitOfWork>> logger) : base(unitOfWork, logger)
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
