using Doitsu.Service.Core.Interfaces.EfCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EFCore.Abstractions.EntityChangeHandlers
{
    public class ActivableHandler : IEntityChangeHandler
    {
        public void Handle(DbContext context)
        {
            context.ChangeTracker.Entries().Where(e => e.Entity is IActivable).ToList()
                .ForEach(entry => {
                    if(entry.State == EntityState.Deleted) {
                        entry.State = EntityState.Modified;
                        ((IActivable)entry.Entity).Active = false;
                    }
                    else if(entry.State == EntityState.Added || entry.State == EntityState.Modified) {
                        ((IActivable)entry.Entity).Active = true;
                    }
                });
        }

        public async Task HandleAsync(DbContext context)
        {
            await Task.Run(() => Handle(context)).ConfigureAwait(false);
        }
    }
}
