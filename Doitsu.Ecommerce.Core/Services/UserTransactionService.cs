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
        UserTransaction PrepareUserTransaction(Orders orders, EcommerceIdentityUser user, UserTransactionTypeEnum type = UserTransactionTypeEnum.Expense);
        Task<Option<EcommerceIdentityUser, string>> UpdateUserBalanceAsync(UserTransaction userTransaction, EcommerceIdentityUser user);
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

        public UserTransaction PrepareUserTransaction(Orders orders, EcommerceIdentityUser user, UserTransactionTypeEnum type = UserTransactionTypeEnum.Expense)
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
                    userTransaction.Description = $"THANH TOÁN đơn hàng {orders.Code}";
                    break;

                case UserTransactionTypeEnum.Income:
                    userTransaction.Description = $"NẠP TIỀN từ đơn hàng {orders.Code}";
                    break;

                case UserTransactionTypeEnum.Rollback:
                    userTransaction.Description = $"HOÀN TIỀN từ đơn hàng {orders.Code}, lý do {orders.Note}";
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
                            if (user.Balance < userTransaction.Amount)
                            {
                                return Option.None<EcommerceIdentityUser, string>("Hiện tại số dư tài khoản của bạn không đủ để thanh toán đơn hàng này.");
                            }
                            user.Balance -= userTransaction.Amount;
                            break;

                        case UserTransactionTypeEnum.Income:
                            user.Balance += userTransaction.Amount;
                            break;

                        case UserTransactionTypeEnum.Rollback:
                            user.Balance += userTransaction.Amount;
                            break;
                    }
                    await this.userManager.UpdateAsync(user);
                    await this.CreateAsync(userTransaction);
                    return Option.Some<EcommerceIdentityUser, string>(user);
                });
        }
    }
}
