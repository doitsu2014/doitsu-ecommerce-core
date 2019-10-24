using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Service.Core;

namespace Doitsu.Ecommerce.Core.Abstraction.Interfaces
{
    public interface IEcommerceUnitOfWork : IUnitOfWork<EcommerceDbContext>
    {
        Task<int> CommitWithoutBeforeSavingAsync();
    }
}
