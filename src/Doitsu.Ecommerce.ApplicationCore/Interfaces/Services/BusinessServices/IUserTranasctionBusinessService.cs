using System.Collections.Immutable;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Optional;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices
{
    public interface IUserTransactionBusinessService
    {
        Task<Option<(int userId, UserTransaction userTransaction), string>> UpdateUserBalanceAsync(Orders order,
                                                                                                   ImmutableList<ProductVariants> productVariants,
                                                                                                   UserTransactionTypeEnum userTransactionType);
    }
}