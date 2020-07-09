using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Extensions;
using Doitsu.Ecommerce.Infrastructure.Extensions;

namespace Doitsu.Ecommerce.ApplicationCore.Tests
{
    public class Startup
    {
        private readonly IDatabaseConfigurer databaseConfigurer;
        public Startup(IConfiguration configuration, IDatabaseConfigurer databaseConfigurer)
        {
            Configuration = configuration;
            this.databaseConfigurer = databaseConfigurer;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDoitsuEcommerceInfrastructure(Configuration, databaseConfigurer);
            services.AddDoitsuEcommerceCore();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDoitsuEcommerceCoreHosting(env);
        }
    }
}