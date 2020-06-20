using Doitsu.Ecommerce.Core.SEO.Interfaces;
using Doitsu.Ecommerce.Core.SEO.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doitsu.Ecommerce.Core.SEO
{
    public static class Extension
    {
        public static void ConfigSeo(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DomainModel>(o => configuration.GetSection("DomainModel").Bind(o));
            services.AddScoped<IBreadcrumbGenerator, BreadcrumbGenerator>();
        }
    }
}
