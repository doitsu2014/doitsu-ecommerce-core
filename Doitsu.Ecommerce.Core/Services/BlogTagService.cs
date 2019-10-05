using System.Linq;
using System.Threading.Tasks;
using Doitsu.Service.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IBlogTagService : IBaseService<BlogTags>
    {
        Task DeleteAllByBlogIdAsync(int blogId);
    }

    public class BlogTagService : BaseService<BlogTags>, IBlogTagService
    {
        public BlogTagService(IUnitOfWork unitOfWork, ILogger<BaseService<BlogTags>> logger) : base(unitOfWork, logger)
        {
        }

        public async Task DeleteAllByBlogIdAsync(int blogId)
        {
            var allBlogTags = await this
                .Get(x => x.BlogId == blogId)
                .Select(x => x.Id)
                .ToListAsync();

            await this.HardDeleteByRangeKeysAsync<int>(allBlogTags);
        }
    }
}
