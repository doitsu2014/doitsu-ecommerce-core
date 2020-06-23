using Doitsu.Service.Core.Interfaces.EfCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EFCore.Abstractions.EntityChangeHandlers
{
    public class SoftDeletableHandler : IEntityChangeHandler
    {
        public void Handle(DbContext context)
        {
            context.ChangeTracker.Entries().Where(e => e.Entity is ISoftDeletable && e.State == EntityState.Deleted)
                .ToList().ForEach(e =>
                {
                    e.State = EntityState.Modified;
                    ((ISoftDeletable)e.Entity).Deleted = true;
                });
        }

        public async Task HandleAsync(DbContext context)
        {
            await Task.Run(() => Handle(context)).ConfigureAwait(false);
        }
    }
}
