using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Doitsu.Service.Core;
using Doitsu.Ecommerce.Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
using System.Collections.Generic;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Ecommerce.Core.IdentitiesExtension;
using Optional;
using Optional.Async;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IUserTransactionService : IBaseService<UserTransaction>
    {
        UserTransaction PrepareUserTransaction(Orders orders, ImmutableList<ProductVariantViewModel> productVariants, EcommerceIdentityUser user, UserTransactionTypeEnum type = UserTransactionTypeEnum.Expense);
        Task<Option<EcommerceIdentityUser, string>> UpdateUserBalanceAsync(UserTransaction userTransaction, EcommerceIdentityUser user);
        Task<ImmutableList<UserTransactionViewModel>> GetByUserIdAsync(int userId);
    }

    /// <summary>
    /// should use entity on this service
    /// </summary>
    public class UserTransactionService : BaseService<UserTransaction>, IUserTransactionService
    {
        private readonly IMemoryCache memoryCache;
        private readonly EcommerceIdentityUserManager<EcommerceIdentityUser> userManager;

        public UserTransactionService(EcommerceDbContext dbContext,
                          IMapper mapper,
                          ILogger<BaseService<UserTransaction, EcommerceDbContext>> logger,
                          IMemoryCache memoryCache,
                          EcommerceIdentityUserManager<EcommerceIdentityUser> userManager) : base(dbContext, mapper, logger)
        {
            this.memoryCache = memoryCache;
            this.userManager = userManager;
        }


        public UserTransaction PrepareUserTransaction(Orders orders, ImmutableList<ProductVariantViewModel> productVariants, EcommerceIdentityUser user, UserTransactionTypeEnum type = UserTransactionTypeEnum.Expense)
        {
            var userTransaction = new UserTransaction()
            {
                Type = type,
                Amount = orders.FinalPrice,
                CreatedTime = DateTime.UtcNow.ToVietnamDateTime(),
                Order = orders,
                UserId = user.Id
            };

            switch (type)
            {
                case UserTransactionTypeEnum.Expense:
                    userTransaction.Description = $"THANH TOÁN đơn hàng {orders.Code}, ";
                    var optionNameValues = productVariants.Select(pv => {
                       return pv.ProductVariantOptionValues.Select(
                           pvov => $"{pvov.ProductOption.Name}: {pvov.ProductOptionValue.Value}"
                       ).Aggregate((x,y) => $"{x}, {y}");
                    });
                    userTransaction.Description += optionNameValues.Aggregate((x,y) => $"{x}, {y}");
                    break;

                case UserTransactionTypeEnum.Income:
                    userTransaction.Description = $"NẠP TIỀN từ đơn hàng {orders.Code}, vào tài khoản {user.UserName}.";
                    break;

                case UserTransactionTypeEnum.Rollback:
                    userTransaction.Description = userTransaction.Description.IsNullOrEmpty() 
                        ? $"HOÀN TIỀN từ đơn hàng {orders.Code}" 
                        : userTransaction.Description;
                    break;

                case UserTransactionTypeEnum.Withdrawal:
                    userTransaction.Description = userTransaction.Description.IsNullOrEmpty()
                        ? $"RÚT TIỀN từ đơn hàng {orders.Code}"
                        : userTransaction.Description;
                    break;
            }

            return userTransaction;
        }

        public async Task<Option<EcommerceIdentityUser, string>> UpdateUserBalanceAsync(UserTransaction userTransaction, EcommerceIdentityUser user)
        {
            return await (userTransaction, user).SomeNotNull()
                .WithException(string.Empty)
                .FlatMapAsync(async d =>
                {
                    switch (userTransaction.Type)
                    {
                        case UserTransactionTypeEnum.Expense:
                        case UserTransactionTypeEnum.Withdrawal:
                            if (user.Balance < userTransaction.Amount)
                            {
                                return Option.None<EcommerceIdentityUser, string>("Hiện tại số dư tài khoản của bạn không đủ để thanh toán đơn hàng này.");
                            }
                            userTransaction.CurrentBalance = user.Balance;
                            user.Balance -= userTransaction.Amount;
                            userTransaction.DestinationBalance = user.Balance;
                            userTransaction.Sign = UserTransactionSignEnum.Substract;
                            break;
                        case UserTransactionTypeEnum.Income:
                        case UserTransactionTypeEnum.Rollback:
                            userTransaction.CurrentBalance = user.Balance;
                            user.Balance += userTransaction.Amount;
                            userTransaction.DestinationBalance = user.Balance;
                            userTransaction.Sign = UserTransactionSignEnum.Plus;
                            break;
                    }
                    await this.userManager.UpdateAsync(user);
                    await this.CreateAsync(userTransaction);
                    return Option.Some<EcommerceIdentityUser, string>(user);
                });
        }

        public async Task<ImmutableList<UserTransactionViewModel>> GetByUserIdAsync(int userId)
        {
            return (await this.DbContext.UserTransactions.Where(ut => ut.UserId == userId)
                .ProjectTo<UserTransactionViewModel>(this.Mapper.ConfigurationProvider)
                .OrderByDescending(ut => ut.CreatedTime)
                .ToListAsync()).ToImmutableList();
        }
    }
}
