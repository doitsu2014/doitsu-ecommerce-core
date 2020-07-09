using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Doitsu.Ecommerce.ApplicationCore
{
    public class DoitsuControllerBase<TService> : ControllerBase
    {
        protected readonly TService baseService;
        protected readonly IServiceProvider serviceProvider;
        protected readonly ILogger<DoitsuControllerBase<TService>> logger;
        public DoitsuControllerBase(TService baseService, IServiceProvider serviceProvider, ILogger<DoitsuControllerBase<TService>> logger)
        {
            this.baseService = baseService;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }
    }
}