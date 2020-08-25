using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.OrderSpecifications
{
    public class OrderFilterByOrderCodeSpecification : BaseSpecification<Orders>
    {
        public OrderFilterByOrderCodeSpecification(string orderCode) : base(o => o.Code == orderCode)
        {
            AddInclude(o => o.UserTransactions);
            AddIncludes(
                o => o.Include(qO => qO.OrderItems).ThenInclude(qO => qO.Product)
                        .Include(qO => qO.OrderItems).ThenInclude(qO => qO.ProductVariant)
            );
        }
    }
}