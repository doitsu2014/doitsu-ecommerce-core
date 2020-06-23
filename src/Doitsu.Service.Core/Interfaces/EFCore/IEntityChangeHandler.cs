using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Doitsu.Service.Core.Interfaces.EfCore
{
    public interface IEntityChangeHandler
    {
        void Handle(DbContext context);
        Task HandleAsync(DbContext context);
    }
}
