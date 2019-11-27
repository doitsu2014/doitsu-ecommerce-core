using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IBlogCategoryService : IBaseService<BlogCategories>
    {

    }

    public class BlogCategoryService : BaseService<BlogCategories>, IBlogCategoryService
    {
        public BlogCategoryService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<BlogCategories, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
