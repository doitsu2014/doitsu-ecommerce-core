using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IPromotionDetailService : IBaseService<PromotionDetail>
    {
    }

    public class PromotionDetailService : BaseService<PromotionDetail>, IPromotionDetailService
    {
        public PromotionDetailService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<PromotionDetail, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
