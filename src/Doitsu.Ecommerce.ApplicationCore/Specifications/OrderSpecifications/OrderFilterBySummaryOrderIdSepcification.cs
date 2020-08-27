using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.OrderSpecifications
{
    public class OrderFilterBySummaryOrderIdSepcification : BaseSpecification<Orders>
    {
        public OrderFilterBySummaryOrderIdSepcification(int summaryOrderId) 
            : base(o => o.SummaryOrderId == summaryOrderId)
        {
            AddIncludes(o => 
                o.Include(qO => qO.SummaryOrder)
            );
        }
    }
}