using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.OrderSpecifications
{
    public class OrderSummaryFilterByIdSpecification : BaseSpecification<Orders>
    {
        public OrderSummaryFilterByIdSpecification(int id) : base(o => o.Id == id && o.Type == OrderTypeEnum.Summary)
        {
            AddIncludes(
                o => o.Include(qO => qO.InverseSummaryOrders)
                        .ThenInclude(qO => qO.OrderItems)
                            .ThenInclude(qO => qO.Product)
                      .Include(qO => qO.InverseSummaryOrders)
                        .ThenInclude(qO => qO.OrderItems)
                            .ThenInclude(qO => qO.ProductVariant)
            );
        }
    }
}