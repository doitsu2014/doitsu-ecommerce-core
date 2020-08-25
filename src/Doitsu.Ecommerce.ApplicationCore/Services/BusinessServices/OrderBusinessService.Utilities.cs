using Doitsu.Ecommerce.ApplicationCore.Entities;

namespace Doitsu.Ecommerce.ApplicationCore.Services.BusinessServices
{
    public partial class OrderBusinessService
    {
        private UserTransaction PrepareUserTransaction()
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
                    var optionNameValues = productVariants.Select(pv =>
                    {
                        return pv.ProductVariantOptionValues.Select(
                            pvov => $"{pvov.ProductOption.Name}: {pvov.ProductOptionValue.Value}"
                        ).Aggregate((x, y) => $"{x}, {y}");
                    });
                    userTransaction.Description += optionNameValues.Aggregate((x, y) => $"{x}, {y}");
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

    }
}