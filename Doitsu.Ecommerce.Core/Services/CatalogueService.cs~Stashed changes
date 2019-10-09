using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface ICatalogueService : IBaseService<Catalogues>
    {
    }

    public class CatalogueService : BaseService<Catalogues>, ICatalogueService
    {
        public CatalogueService(IUnitOfWork unitOfWork, ILogger<BaseService<Catalogues>> logger) : base(unitOfWork, logger)
        {
        }
    }
}
