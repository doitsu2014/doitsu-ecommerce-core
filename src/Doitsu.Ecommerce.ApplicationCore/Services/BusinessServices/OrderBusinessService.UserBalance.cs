using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Optional;
using Optional.Async;

namespace Doitsu.Ecommerce.ApplicationCore.Services.BusinessServices
{
    public partial class OrderBusinessService
    {
        private async Task<Option<(int userId, UserTransaction userTransaction), string>> UpdateUserBalanceAsync(Orders order, ImmutableList<ProductVariants> productVariants, UserTransactionTypeEnum userTransactionType)
        {
            return await (order, productVariants, userTransactionType).SomeNotNull()
                .WithException(string.Empty)
                .FlatMap(d => PrepareUserTransaction(d.order, d.productVariants, d.userTransactionType))
                .MapAsync(async d => (userTransaction: d, user: await this.userManager.FindByIdAsync(d.UserId.ToString())))
                .FlatMapAsync(async d =>
                {
                    switch (d.userTransaction.Type)
                    {
                        case UserTransactionTypeEnum.Expense:
                        case UserTransactionTypeEnum.Withdrawal:
                            if (d.user.Balance < d.userTransaction.Amount)
                            {
                                return Option.None<(int userId, UserTransaction userTransaction), string>("Hiện tại số dư tài khoản của bạn không đủ để thanh toán đơn hàng này.");
                            }
                            d.userTransaction.CurrentBalance = d.user.Balance;
                            d.user.Balance -= d.userTransaction.Amount;
                            d.userTransaction.DestinationBalance = d.user.Balance;
                            d.userTransaction.Sign = UserTransactionSignEnum.Substract;
                            break;
                        case UserTransactionTypeEnum.Income:
                        case UserTransactionTypeEnum.Rollback:
                            d.userTransaction.CurrentBalance = d.user.Balance;
                            d.user.Balance += d.userTransaction.Amount;
                            d.userTransaction.DestinationBalance = d.user.Balance;
                            d.userTransaction.Sign = UserTransactionSignEnum.Plus;
                            break;
                    }
                    await this.userManager.UpdateAsync(d.user);
                    await this.userTransactionRepository.AddAsync(d.userTransaction);
                    return Option.Some<(int userId, UserTransaction userTransaction), string>((d.user.Id, d.userTransaction));
                });
        }

        private Option<UserTransaction, string> PrepareUserTransaction(Orders order, ImmutableList<ProductVariants> productVariants, UserTransactionTypeEnum userTransactionType)
        {
            return (order, productVariants, userTransactionType)
                .SomeNotNull()
                .WithException(string.Empty)
                .Map(d => (d.order, d.productVariants, userTransaction: new UserTransaction()
                {
                    Type = d.userTransactionType,
                    Amount = d.order.FinalPrice,
                    CreatedTime = DateTime.UtcNow.ToVietnamDateTime(),
                    OrderId = order.Id,
                    UserId = order.UserId
                }))
                .Map(d =>
                {
                    switch (userTransactionType)
                    {
                        case UserTransactionTypeEnum.Expense:
                            d.userTransaction.Description = $"THANH TOÁN đơn hàng {d.order.Code}, ";
                            d.userTransaction.Description += d.productVariants.Select(pv =>
                            {
                                return pv.ProductVariantOptionValues.Select(
                                    pvov => $"{pvov.ProductOption.Name}: {pvov.ProductOptionValue.Value}"
                                ).Aggregate((x, y) => $"{x}, {y}");
                            }).Aggregate((x, y) => $"{x}, {y}");
                            break;
                        case UserTransactionTypeEnum.Income:
                            d.userTransaction.Description = $"NẠP TIỀN vào tài khoản từ đơn hàng {order.Code}.";
                            break;
                        case UserTransactionTypeEnum.Rollback:
                            d.userTransaction.Description = d.userTransaction.Description.IsNullOrEmpty()
                                ? $"HOÀN TIỀN từ đơn hàng {order.Code}"
                                : d.userTransaction.Description;
                            break;
                        case UserTransactionTypeEnum.Withdrawal:
                            d.userTransaction.Description = d.userTransaction.Description.IsNullOrEmpty()
                                ? $"RÚT TIỀN từ đơn hàng {order.Code}"
                                : d.userTransaction.Description;
                            break;
                    }
                    return d.userTransaction;
                });
        }
    }
}