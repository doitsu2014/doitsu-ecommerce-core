using Doitsu.Ecommerce.Core.Abstraction.Entities;
using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IBlogCategoryService : IEcommerceBaseService<BlogCategories>
    {

    }

    public class BlogCategoryService : EcommerceBaseService<BlogCategories>, IBlogCategoryService
    {
        public BlogCategoryService(EcommerceDbContext dbContext, IMapper mapper, ILogger<EcommerceBaseService<BlogCategories>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
