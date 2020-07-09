using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data.Handlers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityChangeHandlers
{
    /// <summary>
    /// Work-around for Timestamp issue of EF Core 2.1.x:
    /// - In disconected mode, timestamp must be updated to the value when this entity was fetched, doing this
    /// will guarantee a DbUpdateConcurrencyException is thrown if this entity has been updated since then.
    /// </summary>
    public class TimestampWorkaroundHandler : IEntityChangeHandler
    {
        public void Handle(DbContext context)
        {
            context.ChangeTracker.Entries().Where(e => e.Entity is Entity && e.State != EntityState.Unchanged)
                .ToList().ForEach(e =>
                {
                    var timestampProperty = e.Property("Timestamp");
                    timestampProperty.OriginalValue = timestampProperty.CurrentValue;
                });
        }

        public async Task HandleAsync(DbContext context)
        {
            await Task.Run(() => Handle(context)).ConfigureAwait(false);
        }
    }
}
