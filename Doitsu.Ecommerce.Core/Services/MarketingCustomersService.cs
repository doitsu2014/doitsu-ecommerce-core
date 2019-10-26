using System.Threading.Tasks;
using Doitsu.Service.Core;
using Doitsu.Ecommerce.Core.ViewModels;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IMarketingCustomerService : IBaseService<MarketingCustomers>
    {

        Task<MarketingCustomers> CreateWithConstraintAsync(MarketingCustomerViewModel data, int userId);

    }

    public class MarketingCustomerService : BaseService<MarketingCustomers>, IMarketingCustomerService
    {
        public MarketingCustomerService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<MarketingCustomers, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }

        public async Task<MarketingCustomers> CreateWithConstraintAsync(MarketingCustomerViewModel data, int userId)
        {
            var exist = await this.FirstOrDefaultAsync(x => x.Email == data.Email);

            if (exist != null)
            {
                if (userId > 0)
                {
                    exist.UserId = userId;
                }
                this.Update(exist);
                await CommitAsync();
                return exist;
            }
            else
            {
                var result = await this.CreateAsync<MarketingCustomerViewModel>(data);
                if (userId > 0)
                {
                    result.UserId = userId;
                }
                await CommitAsync();
                return result;
            }
        }
    }
}
