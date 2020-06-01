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
using Doitsu.Service.Core.Interfaces.EfCore;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Service.Core.Interfaces;
using Doitsu.Ecommerce.Core.DatabaseConfigurer;

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


        private static DbContextOptionsBuilder<T> GenerateOptionsBuilderForRealDb<T>(string connectionString, IServiceProvider serviceProvider, string assemblyName) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlServer(connectionString, builder => builder.MigrationsAssembly(assemblyName));
            builder.UseLoggerFactory(serviceProvider.GetService<ILoggerFactory>());
            return builder;
        }

        private static DbContextOptionsBuilder<T> GenerateOptionsBuilderForInMemoryDb<T>(string connectionString, IServiceProvider serviceProvider, string assemblyName) where T : DbContext
        {
            var builder = new DbContextOptionsBuilder<T>();
            builder.UseSqlite(connectionString, builder => builder.MigrationsAssembly(assemblyName));
            builder.UseLoggerFactory(serviceProvider.GetService<ILoggerFactory>());
            return builder;
        }

        private static IWebHostBuilder StandardWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>();
        }

        private static IWebHostBuilder BuildWebhostWithJsonContent()
        {
            return StandardWebHostBuilder()
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

        public static IWebHostBuilder BuilderWebhostWithRealDb(string poolKey)
        {
            return BuildWebhostWithJsonContent()
                .ConfigureServices(services => services.AddSingleton<IDatabaseConfigurer, EcommerceDatabaseConfigurer>())
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString(nameof(EcommerceDbContext));
                    var generatedPoolKeyConnStr = DatabaseHelper.GeneratePoolKeyConnectionString(connectionString, poolKey);
                    services.AddScoped(serviceProvider =>
                    {
                        var connectionString = generatedPoolKeyConnStr;
                        var opts = GenerateOptionsBuilderForRealDb<EcommerceDbContext>(connectionString, serviceProvider, typeof(EcommerceDbContext).Assembly.GetName().Name).Options;
                        var dbContext = new Data.EcommerceDbContext(opts, serviceProvider.GetService<IEnumerable<IEntityChangeHandler>>());
                        return dbContext;
                    });
                });
        }

        public static IWebHostBuilder BuilderWebhostWithInmemoryDb(string poolKey)
        {
            return BuildWebhostWithJsonContent()
                .ConfigureServices(services => services.AddSingleton<IDatabaseConfigurer, InMemorySqliteConfigurer>());
        }
    }
}
