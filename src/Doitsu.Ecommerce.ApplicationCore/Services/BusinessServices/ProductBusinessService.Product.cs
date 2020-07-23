using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices;
using Microsoft.Extensions.Logging;
using Optional;
using Optional.Async;

namespace Doitsu.Ecommerce.ApplicationCore.Services.BusinessServices
{
    public partial class ProductBusinessService : IProductService
    {
        public async Task<Option<int, string>> CreateProductWithOptionAsync(Products data)
        {
            return await data.SomeNotNull()
                .WithException("Dữ liệu truyền vào bị rỗng")
                .MapAsync(async d =>
                {
                    using (var transaction = await databaseManager.GetDatabaseContextTransactionAsync())
                    {
                        data.ProductVariants = this.BuildListProductVariant(data.Id, data.Code, data.ProductOptions.ToArray(), data.ProductVariants.ToArray());
                        var created = await productRepository.AddAsync(data);
                        await transaction.CommitAsync();
                        return created.Id;
                    }
                });
        }

        public async Task<Option<int[], string>> CreateProductWithOptionAsync(Products[] data)
        {
            using (var transaction = await databaseManager.GetDatabaseContextTransactionAsync())
            {
                return await data.SomeNotNull()
                    .WithException("Dữ liệu truyền vào bị rỗng")
                    .MapAsync(async d =>
                    {
                        var result = new int[] {};
                        foreach (var product in d)
                        {
                            product.ProductVariants = this.BuildListProductVariant(product.Id, product.Code, product.ProductOptions.ToArray(), product.ProductVariants.ToArray());
                            var created = await productRepository.AddAsync(product);
                            result = result.Append(created.Id).ToArray();
                        }

                        await transaction.CommitAsync();
                        return result;
                    });
            }
        }

    }
}