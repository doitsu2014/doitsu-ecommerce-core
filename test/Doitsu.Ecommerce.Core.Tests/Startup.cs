
using Doitsu.Ecommerce.Core.DeliveryIntegration;
using Doitsu.Service.Core.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Doitsu.Ecommerce.Core.Tests
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
            services.AddDoitsuEcommerceCore(Configuration, databaseConfigurer, true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseDoitsuEcommerceCoreHosting(env,true);
        }
    }
}