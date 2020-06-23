using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IProductOptionService : IEcommerceBaseService<ProductOptions>
    {
    }

    public class ProductOptionService : EcommerceBaseService<ProductOptions>, IProductOptionService
    {
        public ProductOptionService(EcommerceDbContext dbContext, IMapper mapper, ILogger<EcommerceBaseService<ProductOptions>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
