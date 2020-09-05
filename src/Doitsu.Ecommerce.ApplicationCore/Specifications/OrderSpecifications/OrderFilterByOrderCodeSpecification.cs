using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.OrderSpecifications
{
    public class OrderFilterByOrderCodeSpecification : BaseSpecification<Orders>
    {
        public OrderFilterByOrderCodeSpecification(string orderCode) : base(o => o.Code == orderCode && o.Type != OrderTypeEnum.Summary)
        {
            AddInclude(o => o.UserTransactions);
            AddIncludes(
                o => o.Include(qO => qO.OrderItems).ThenInclude(qO => qO.Product)
                        .Include(qO => qO.OrderItems).ThenInclude(qO => qO.ProductVariant)
            );
        }

        public OrderFilterByOrderCodeSpecification(string orderCode, OrderTypeEnum orderTypeEnum) : base(o => o.Code == orderCode && o.Type == orderTypeEnum)
        {
        }
    }
}