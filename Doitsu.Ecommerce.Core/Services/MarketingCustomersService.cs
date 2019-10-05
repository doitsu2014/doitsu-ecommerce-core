using System.Threading.Tasks;
using Doitsu.Service.Core;
using Doitsu.Ecommerce.Core.ViewModels;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IMarketingCustomerService : IBaseService<MarketingCustomers>
    {

        Task<MarketingCustomers> CreateWithConstraintAsync(MarketingCustomerViewModel data, int userId);

    }

    public class MarketingCustomerService : BaseService<MarketingCustomers>, IMarketingCustomerService
    {
        public MarketingCustomerService(IUnitOfWork unitOfWork, ILogger<BaseService<MarketingCustomers>> logger) : base(unitOfWork, logger)
        {

        }

        public async Task<MarketingCustomers> CreateWithConstraintAsync(MarketingCustomerViewModel data, int userId)
        {
            var exist = await this.FirstOrDefaultActiveAsync(x => x.Email == data.Email);

            if (exist != null)
            {
                if (userId > 0)
                {
                    exist.UserId = userId;
                }
                this.Update(exist);
                await this.UnitOfWork.CommitAsync();
                return exist;
            }
            else
            {
                var result = await this.CreateAsync<MarketingCustomerViewModel>(data);
                if (userId > 0)
                {
                    result.UserId = userId;
                }
                await this.UnitOfWork.CommitAsync();
                return result;
            }
        }
    }
}
