using System;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Service.Core.Interfaces.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCore.Abstractions.EntityChangeHandlers
{
    public class AuditableHandler : IEntityChangeHandler
    {
        private readonly ILogger<AuditableHandler> _logger;

        public AuditableHandler(ILogger<AuditableHandler> logger)
        {
            _logger = logger;
        }

        public void Handle(DbContext context)
        {
            context.ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditable)
                .ToList().ForEach(e =>
                {
                    var entity = e.Entity as IAuditable;
                    if (e.State == EntityState.Added)
                    {
                        entity.CreatedDate = DateTime.Now;
                    }
                    else if (e.State == EntityState.Modified)
                    {
                        entity.LastUpdatedDate = DateTime.Now;
                    }
                });
        }

        public async Task HandleAsync(DbContext context)
        {
            await Task.Run(() => Handle(context)).ConfigureAwait(false);
        }
    }
}
