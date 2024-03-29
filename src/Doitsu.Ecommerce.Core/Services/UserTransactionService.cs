﻿using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Doitsu.Service.Core;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;

using Doitsu.Ecommerce.Core.Abstraction;
using System.Collections.Generic;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Abstraction.Identities;
using Doitsu.Ecommerce.Core.IdentitiesExtension;
using Optional;
using Optional.Async;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IUserTransactionService : IEcommerceBaseService<UserTransaction>
    {
        UserTransaction PrepareUserTransaction(OrderDetailViewModel orders, ImmutableList<ProductVariantViewModel> productVariants, int userId, UserTransactionTypeEnum type = UserTransactionTypeEnum.Expense);
        Task<Option<EcommerceIdentityUser, string>> UpdateUserBalanceAsync(UserTransaction userTransaction, int userId);
        Task<ImmutableList<UserTransactionViewModel>> GetByUserIdAsync(int userId);
    }

    /// <summary>
    /// should use entity on this service
    /// </summary>
    public class UserTransactionService : EcommerceBaseService<UserTransaction>, IUserTransactionService
    {
        private readonly EcommerceIdentityUserManager<EcommerceIdentityUser> userManager;

        public UserTransactionService(EcommerceDbContext dbContext,
                          IMapper mapper,
                          ILogger<EcommerceBaseService<UserTransaction>> logger,
                          EcommerceIdentityUserManager<EcommerceIdentityUser> userManager) : base(dbContext, mapper, logger)
        {
            this.userManager = userManager;
        }


        public UserTransaction PrepareUserTransaction(OrderDetailViewModel order, ImmutableList<ProductVariantViewModel> productVariants, int userId, UserTransactionTypeEnum type = UserTransactionTypeEnum.Expense)
        {
            var userTransaction = new UserTransaction()
            {
                Type = type,
                Amount = order.FinalPrice,
                CreatedTime = DateTime.UtcNow.ToVietnamDateTime(),
                OrderId = order.Id,
                UserId = userId 
            };

            switch (type)
            {
                case UserTransactionTypeEnum.Expense:
                    userTransaction.Description = $"THANH TOÁN đơn hàng {order.Code}, ";
                    var optionNameValues = productVariants.Select(pv => {
                       return pv.ProductVariantOptionValues.Select(
                           pvov => $"{pvov.ProductOption.Name}: {pvov.ProductOptionValue.Value}"
                       ).Aggregate((x,y) => $"{x}, {y}");
                    });
                    userTransaction.Description += optionNameValues.Aggregate((x,y) => $"{x}, {y}");
                    break;

                case UserTransactionTypeEnum.Income:
                    userTransaction.Description = $"NẠP TIỀN vào tài khoản từ đơn hàng {order.Code}.";
                    break;

                case UserTransactionTypeEnum.Rollback:
                    userTransaction.Description = userTransaction.Description.IsNullOrEmpty() 
                        ? $"HOÀN TIỀN từ đơn hàng {order.Code}" 
                        : userTransaction.Description;
                    break;

                case UserTransactionTypeEnum.Withdrawal:
                    userTransaction.Description = userTransaction.Description.IsNullOrEmpty()
                        ? $"RÚT TIỀN từ đơn hàng {order.Code}"
                        : userTransaction.Description;
                    break;
            }

            return userTransaction;
        }

        public async Task<Option<EcommerceIdentityUser, string>> UpdateUserBalanceAsync(UserTransaction userTransaction, int userId)
        {
            return await (userTransaction, userId).SomeNotNull()
                .WithException(string.Empty)
                .MapAsync(async d => (userTransaction: d.userTransaction, user: await this.userManager.FindByIdAsync(userId.ToString())))
                .FlatMapAsync(async d =>
                {
                    switch (userTransaction.Type)
                    {
                        case UserTransactionTypeEnum.Expense:
                        case UserTransactionTypeEnum.Withdrawal:
                            if (d.user.Balance < userTransaction.Amount)
                            {
                                return Option.None<EcommerceIdentityUser, string>("Hiện tại số dư tài khoản của bạn không đủ để thanh toán đơn hàng này.");
                            }
                            d.userTransaction.CurrentBalance = d.user.Balance;
                            d.user.Balance -= userTransaction.Amount;
                            d.userTransaction.DestinationBalance = d.user.Balance;
                            d.userTransaction.Sign = UserTransactionSignEnum.Substract;
                            break;
                        case UserTransactionTypeEnum.Income:
                        case UserTransactionTypeEnum.Rollback:
                            d.userTransaction.CurrentBalance = d.user.Balance;
                            d.user.Balance += userTransaction.Amount;
                            d.userTransaction.DestinationBalance = d.user.Balance;
                            d.userTransaction.Sign = UserTransactionSignEnum.Plus;
                            break;
                    }
                    await this.userManager.UpdateAsync(d.user);
                    await this.CreateAsync(userTransaction);
                    return Option.Some<EcommerceIdentityUser, string>(d.user);
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
