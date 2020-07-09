using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Data.Handlers
{
    public interface IEntityChangeHandler
    {
        void Handle(DbContext context);
        Task HandleAsync(DbContext context);
    }
}
