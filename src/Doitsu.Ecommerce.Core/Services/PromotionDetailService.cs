﻿using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IPromotionDetailService : IEcommerceBaseService<PromotionDetail>
    {
    }

    public class PromotionDetailService : EcommerceBaseService<PromotionDetail>, IPromotionDetailService
    {
        public PromotionDetailService(EcommerceDbContext dbContext, IMapper mapper, ILogger<EcommerceBaseService<PromotionDetail>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
