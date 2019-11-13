using Doitsu.Ecommerce.Core.Tests.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Optional;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Service.Core.Utils;
using Doitsu.Service.Core.Interfaces.EfCore;

namespace Doitsu.Ecommerce.Core.Tests.Helpers
{
    public static class WebHostBuilderHelper
    {
        private static string GenerateRandomConnectionString(WebHostBuilderContext context, string connectionStringName)
        {
            var parts = context.Configuration.GetConnectionString(connectionStringName).Split(';').ToList();
            var database = parts.Single(p => p.StartsWith("Database="));
            parts.Remove(database);
            var rand = new Random();
            var suffix = DateTime.Now.ToString("yyyyMMddhhmmssfff") + "-" +
                Enumerable.Range(0, 4).Select(i => rand.Next(0, 9).ToString()).Aggregate((x, y) => $"{x}{y}");
            var newDatabase = $"{database}-{suffix}";
            parts.Add(newDatabase);
            return parts.Aggregate((x, y) => $"{x};{y}");
        }

   
        private static DbContextOptionsBuilder<T> GenerateOptions<T>(string connectionString, IServiceProvider serviceProvider, string assemblyName) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlServer(connectionString,
                builder => builder.MigrationsAssembly(assemblyName));
            builder.UseLoggerFactory(serviceProvider.GetService<ILoggerFactory>());
            return builder;
        }

        [Obsolete]
        public static IWebHostBuilder StandardBuilderDb()
        {
            return StandardWebHostBuilderInternal()
                .ConfigureServices(services => services.AddSingleton<IDatabaseConfigurer, TestDatabaseConfigurer>())
                .ConfigureServices((context, services) =>
                {
                    var connectionString = GenerateRandomConnectionString(context, Constants.UnitTestDatabase);
                    services.AddScoped(serviceProvider => connectionString
                        .Map(cs => GenerateOptions<EcommerceDbContext>(cs, serviceProvider, typeof(EcommerceDbContext).Assembly.GetName().Name).Options)
                        .Map(opts => new EcommerceDbContext(opts, serviceProvider.GetService<IEnumerable<IEntityChangeHandler>>())));
                    RootConfig.Service(services, context.Configuration, false);
                });
        }

        [Obsolete]
        public static IWebHostBuilder PoolBuilderDb(string poolKey)
        {
            return StandardWebHostBuilderInternal()
                .ConfigureServices(services => services.AddSingleton<IDatabaseConfigurer, TestDatabaseConfigurer>())
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString(Constants.UnitTestDatabase);
                    var generatedPoolKeyConnStr = DatabaseHelper.GeneratePoolKeyConnectionString(connectionString, poolKey);

                    services.AddScoped(serviceProvider => generatedPoolKeyConnStr.Some()
                        .Map(cs => GenerateOptions<EcommerceDbContext>(cs, serviceProvider, typeof(EcommerceDbContext).Assembly.GetName().Name).Options)
                        .Map(opts => new EcommerceDbContext(opts, serviceProvider.GetService<IEnumerable<IEntityChangeHandler>>()))
                        .ValueOr(new EcommerceDbContext(null)));
                    RootConfig.Service(services, context.Configuration, false);
                });
        }

        [Obsolete]
        public static IWebHostBuilder StandardBuilderInMemory()
        {
            return StandardWebHostBuilderInternal()
                .ConfigureServices(services => services.AddSingleton<IDatabaseConfigurer, InMemorySqliteConfigurer>())
                 .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString(Constants.UnitTestDatabase);
                    services.AddScoped(serviceProvider => connectionString.Some()
                        .Map(cs => GenerateOptions<EcommerceDbContext>(cs, serviceProvider, typeof(EcommerceDbContext).Assembly.GetName().Name).Options)
                        .Map(opts => new EcommerceDbContext(opts, serviceProvider.GetService<IEnumerable<IEntityChangeHandler>>()))
                        .ValueOr(new EcommerceDbContext(null)));
                    RootConfig.Service(services, context.Configuration, false);
                });
        }

        private static IWebHostBuilder StandardWebHostBuilderInternal()
        {
            return WebHost.CreateDefaultBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, config) =>
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Configurations");
                    config
                        .AddJsonFile($"{path}/appsettings.json", false, true)
                        .AddJsonFile($"{path}/appsettings." + context.HostingEnvironment.EnvironmentName + ".json", true, true)
                        .AddEnvironmentVariables();
                })
                .ConfigureLogging((context, builder) => builder.ClearProviders())
                .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
        }
    }
}
