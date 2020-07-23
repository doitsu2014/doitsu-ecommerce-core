using System.Collections.Immutable;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Optional;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices
{
    public interface IProductService
    {
        ImmutableList<ProductVariants> BuildListProductVariant(Products product);
        Task<Option<int, string>> UpdateProductVariantAnotherDiscountAsync(int productId, int productVariantId, float anotherDiscount);
        Task<Option<int, string>> UpdateProductVariantAnotherPriceAsync(int productId, int productVariantId, decimal anotherPrice);
        Task<Option<int, string>> UpdateProductVariantStatusAsync(int productId, int productVariantId, ProductVariantStatusEnum value);
        Task<Option<int, string>> UpdateProductVariantInventoryStatusAsync(int productId, int productVariantId, ProductVariantInventoryStatusEnum value);
        Task<Option<int, string>> IncreaseInventoryQuantityAsync(int productId, int productVariantId, int quantity = 0);
        Task<Option<int, string>> DecreaseInventoryQuantityAsync(int productId, int productVariantId, int quantity = 0);
        Task<Option<int[], string>> IncreaseBatchPvInventoryQuantityAsync((int productVariantId, int quantity)[] listProductIdAndQuantity);
        Task<Option<int[], string>> DecreaseBatchPvInventoryQuantityAsync((int productVariantId, int quantity)[] listProductIdAndQuantity); 
    }
}