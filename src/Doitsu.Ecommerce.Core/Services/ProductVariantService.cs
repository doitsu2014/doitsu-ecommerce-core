using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IProductVariantService : IBaseService<ProductVariants>
    {
    }

    public class ProductVariantService : BaseService<ProductVariants>, IProductVariantService
    {
        public ProductVariantService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<ProductVariants, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
