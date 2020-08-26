using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.OrderSpecifications
{
    public class OrderFilterBySummaryOrderIdSepcification : BaseSpecification<Orders>
    {
        public OrderFilterBySummaryOrderIdSepcification(int id) : base(o => o.Id == id && o.Type == OrderTypeEnum.Summary)
        {
            AddIncludes(o => 
                o.Include(qO => qO.SummaryOrder)
            );
        }
    }
}