using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.OrderSpecifications
{
    public class OrderFilterByIdSpecification : BaseSpecification<Orders>
    {
       public OrderFilterByIdSpecification(int id) : base(o => o.Id == id)
        {
            AddInclude(o => o.UserTransactions);
            AddIncludes(
                o => o.Include(qO => qO.OrderItems).ThenInclude(qO => qO.Product)
                        .Include(qO => qO.OrderItems).ThenInclude(qO => qO.ProductVariant)
            );
        } 
    }
}