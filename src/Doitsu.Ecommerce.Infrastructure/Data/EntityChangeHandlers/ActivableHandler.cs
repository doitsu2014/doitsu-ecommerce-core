using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data.Handlers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityChangeHandlers
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
