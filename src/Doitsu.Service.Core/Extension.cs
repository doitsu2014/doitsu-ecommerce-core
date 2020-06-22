using System.Globalization;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.EntityChangeHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Doitsu.Service.Core
{
    public static class Extension
    {
        public static void RegisterDefaultEntityChangesHandlers(this IServiceCollection services)
        {
            services.AddScoped(typeof(IEntityChangeHandler), typeof(ActivableHandler));
            services.AddScoped(typeof(IEntityChangeHandler), typeof(VersWorkaroundHandler));
            services.AddScoped(typeof(IEntityChangeHandler), typeof(AuditableHandler));
            //services.AddScoped(typeof(IEntityChangeHandler), typeof(SoftDeletableHandler));
            //services.AddScoped(typeof(IEntityChangeHandler), typeof(TimestampWorkaroundHandler));
        }

        public static string GetVietnamDong(this decimal value)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
            return value.ToString("#,###", cul.NumberFormat);
        }
    }
}
