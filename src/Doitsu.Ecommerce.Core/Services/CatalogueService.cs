using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface ICatalogueService : IBaseService<Catalogues>
    {
    }

    public class CatalogueService : BaseService<Catalogues>, ICatalogueService
    {
        public CatalogueService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<Catalogues, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
