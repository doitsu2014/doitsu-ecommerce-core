using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using System.Threading.Tasks;
using Optional;
using Optional.Async;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IProductOptionService : IBaseService<ProductOptions>
    {
    }

    public class ProductOptionService : BaseService<ProductOptions>, IProductOptionService
    {
        public ProductOptionService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<ProductOptions, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
