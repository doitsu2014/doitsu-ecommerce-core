using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.OrderSpecifications
{
    public class OrderFilterByUserIdSpecification : BaseSpecification<Orders>
    {
        public OrderFilterByUserIdSpecification(int userId) : base(o => o.UserId == userId)
        {
            AddInclude(o => o.UserTransactions);
            AddIncludes(
                o => o.Include(qO => qO.OrderItems).ThenInclude(qO => qO.Product)
                        .Include(qO => qO.OrderItems).ThenInclude(qO => qO.ProductVariant)
            );
            ApplyOrderBy(o => o.CreatedDate);
        }
        
    }
}