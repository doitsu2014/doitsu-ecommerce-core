using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
namespace Doitsu.Ecommerce.Core.Services
{
    public interface ICatalogueService : IBaseService<Catalogues>
    {
    }

    public class CatalogueService : BaseService<Catalogues>, ICatalogueService
    {
        public CatalogueService(IEcommerceUnitOfWork unitOfWork, ILogger<BaseService<Catalogues>> logger) : base(unitOfWork, logger)
        {
        }
    }
}
