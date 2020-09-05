using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.OrderSpecifications
{
    public class OrderSummaryFilterByCodeSpecification : BaseSpecification<Orders>
    {
        public OrderSummaryFilterByCodeSpecification(string code) : base(o => o.Code == code && o.Type == OrderTypeEnum.Summary)
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