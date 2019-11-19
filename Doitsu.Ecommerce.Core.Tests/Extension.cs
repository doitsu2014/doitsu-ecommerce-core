
using System;

namespace Doitsu.Ecommerce.Core.Tests
{
    public static class TestExtension
    {
        public static TService GetService<TService>(this IServiceProvider serviceProdiver)
            where TService : class
        {
            var service = (TService) serviceProdiver.GetService(typeof(TService));
            return service;
        }
    }
}