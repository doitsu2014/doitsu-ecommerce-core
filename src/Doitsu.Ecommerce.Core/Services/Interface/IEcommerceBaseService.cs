using Doitsu.Ecommerce.Core.Data;
using Doitsu.Service.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doitsu.Ecommerce.Core.Services.Interface
{
    public interface IEcommerceBaseService<TEntity> : IBaseService<TEntity, EcommerceDbContext>
        where TEntity : class, new()
    {
    }
}
