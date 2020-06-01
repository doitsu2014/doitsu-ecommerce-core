using System;
using System.Collections.Generic;
using Doitsu.Ecommerce.Core.Abstraction;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Doitsu.Ecommerce.Core.IdentityServer4.Data
{
    public class EcommerceIs4PersistedGrantDbContext : PersistedGrantDbContext 
    {
        public EcommerceIs4PersistedGrantDbContext(DbContextOptions<PersistedGrantDbContext> options, OperationalStoreOptions storeOptions) : base(options, storeOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("is4ps");
        }

    }
}