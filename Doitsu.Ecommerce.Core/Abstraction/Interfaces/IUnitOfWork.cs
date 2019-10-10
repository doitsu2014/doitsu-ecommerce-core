using Doitsu.Ecommerce.Core.Data;
using Doitsu.Service.Core;

namespace Doitsu.Ecommerce.Core.Abstraction.Interfaces
{
    public interface IUnitOfWork : IUnitOfWork<EcommerceDbContext>
    {
    }
}
