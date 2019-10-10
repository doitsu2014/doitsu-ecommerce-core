using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
namespace Doitsu.Ecommerce.Core.Services
{
    public interface IBlogCategoryService : IBaseService<BlogCategories>
    {

    }

    public class BlogCategoryService : BaseService<BlogCategories>, IBlogCategoryService
    {
        public BlogCategoryService(IUnitOfWork unitOfWork, ILogger<BaseService<BlogCategories>> logger) : base(unitOfWork, logger)
        {

        }

    }
}
