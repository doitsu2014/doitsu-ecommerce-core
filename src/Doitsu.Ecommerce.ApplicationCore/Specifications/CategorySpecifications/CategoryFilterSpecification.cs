using System;
using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;
using Doitsu.Ecommerce.ApplicationCore.Specifications;

namespace Specifications.CategorySpecifications
{
    public class CategoryFilterSpecification : BaseSpecification<Categories>
    {
        public CategoryFilterSpecification(TypeOfCategoryEnum type, string slug)
        {
            if (type == TypeOfCategoryEnum.Normal)
            {
                AddInclude(c => c.ParentCate);
                AddInclude(c => c.Products);
                AddCriteria(c => !c.IsFixed && c.ParentCateId != null && c.Slug == slug);
            }
            else if (type == TypeOfCategoryEnum.Parent)
            {
                AddInclude(c => c.ParentCate);
                AddInclude(c => c.InverseParentCate);
                AddInclude(c => c.Products);
                AddCriteria(c => c.IsFixed && c.Slug == slug);
            }
            else
            {
                AddInclude(c => c.InverseParentCate);
                AddIncludes(c => c.Include(cq => cq.InverseParentCate).ThenInclude(ipcq => ipcq.Products));
                AddCriteria(c => c.IsFixed && c.Slug == slug);
            }
        }
    }
}