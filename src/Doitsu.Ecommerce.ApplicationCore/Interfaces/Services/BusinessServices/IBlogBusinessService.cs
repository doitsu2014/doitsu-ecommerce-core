using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices
{
    public interface IBlogBusinessService
    {
        Task<Blogs> CreateBlogWithTagsAsync(Blogs blogEntity, Tag[] listTags);
        Task UpdateBlogWithTagsAsync(Blogs blogEntity, Tag[] listTags);
    }
}