using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data.Handlers;
using Doitsu.Ecommerce.Infrastructure.Tools.DatabaseConfigurer.Converter;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Optional;

namespace Doitsu.Ecommerce.Infrastructure.Data
{
    public class EcommerceDbContext : IdentityDbContext<EcommerceIdentityUser, EcommerceIdentityRole, int>
    {
        private readonly IEnumerable<IEntityChangeHandler> _handlers;

        public DbSet<BlogCategories> BlogCategories { get; set; }

        public DbSet<Blogs> Blogs { get; set; }

        public DbSet<BlogTags> BlogTags { get; set; }

        public DbSet<Brand> Brand { get; set; }

        public DbSet<BrandFeedbacks> BrandFeedbacks { get; set; }

        public DbSet<Catalogues> Catalogues { get; set; }

        public DbSet<Categories> Categories { get; set; }

        public DbSet<CustomerFeedbacks> CustomerFeedbacks { get; set; }

        public DbSet<Galleries> Galleries { get; set; }

        public DbSet<GalleryItems> GalleryItems { get; set; }

        public DbSet<MarketingCustomers> MarketingCustomers { get; set; }

        public DbSet<OrderItems> OrderItems { get; set; }

        public DbSet<Orders> Orders { get; set; }

        public DbSet<Products> Products { get; set; }

        public DbSet<ProductTag> ProductTags { get; set; }

        public DbSet<Sliders> Sliders { get; set; }

        public DbSet<Tag> Tag { get; set; }

        public DbSet<ProductVariants> ProductVariants { get; set; }

        public DbSet<ProductVariantOptionValues> ProductVariantOptionValues { get; set; }

        public DbSet<ProductOptions> ProductOptions { get; set; }

        public DbSet<ProductOptionValues> ProductOptionValues { get; set; }

        public DbSet<PromotionDetail> PromotionDetails { get; set; }

        public DbSet<UserTransaction> UserTransactions { get; set; }

        public DbSet<WareHouse> WareHouses { get; set; }

        public DbSet<DeliveryInformation> DeliveryInformation { get; set; }


        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options) { }
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options, IEnumerable<IEntityChangeHandler> handlers) : base(options)
        {
            _handlers = handlers;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<EcommerceIdentityUser>()
                .HasMany(e => e.UserRoles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            // Configure for sqlite
            if (this.Database.IsSqlite())
            {
                // Config all rowvers properties to sqlite timestamp
                var timestampProperties = builder.Model
                    .GetEntityTypes()
                    .SelectMany(t => t.GetProperties())
                    .Where(p => p.ClrType == typeof(byte[])
                        && p.ValueGenerated == ValueGenerated.OnAddOrUpdate
                        && p.IsConcurrencyToken);

                foreach (var property in timestampProperties)
                {
                    property.SetValueConverter(new SqliteTimestampConverter());
                    property.SetDefaultValueSql("CURRENT_TIMESTAMP");
                }
            }
        }

        public async Task<int> SaveChangesWithoutBeforeSavingAsync(bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = new CancellationToken())
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public Option<object[], string> GetPrimaryKey<T>(T entity) where T : class
        {
            return entity.SomeNotNull().WithException("Entity cannot be null")
                .Map(e =>
                {
                    var entityType = e.GetType();
                    return new { Type = entityType, EfEntityType = Model.FindEntityType(entityType) };
                })
                .Filter(d => d.EfEntityType != null, () => $"Invalid entity type {entity.GetType().FullName}")
                .Map(d => d.EfEntityType.FindPrimaryKey()
                    .Properties.Select(p => d.Type.GetProperty(p.Name)?.GetValue(entity)).ToArray());
        }

        public Option<IProperty[], string> GetPrimaryKeyProperties(Type entityType)
        {
            return entityType.SomeNotNull().WithException("entityType cannot be null")
                .Map(et => Model.FindEntityType(et))
                .NotNull(() => $"Invalid entity type {entityType.FullName}")
                .Map(et => et.FindPrimaryKey().Properties.ToArray());
        }

        protected virtual void OnBeforeSaving()
        {
            if (_handlers != null && _handlers.Any())
            {
                foreach (var handler in _handlers)
                {
                    handler.Handle(this);
                }
            }
        }

        protected virtual ImmutableList<ValidationResult> ExecuteValidation()
        {
            return ChangeTracker.Entries()
                .Where(e => e.Entity is IValidatableObject && (e.State == EntityState.Added || e.State == EntityState.Modified))
                .Select(e =>
                {
                    var result = new List<ValidationResult>();
                    return Validator.TryValidateObject(e.Entity, new ValidationContext(e.Entity), result) ? null : result;
                })
                .Where(r => r != null).SelectMany(r => r).ToImmutableList();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}