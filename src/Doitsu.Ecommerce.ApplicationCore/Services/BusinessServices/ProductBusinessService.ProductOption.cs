using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.BusinessServices;
using Doitsu.Ecommerce.ApplicationCore.Specifications.ProductSpecifications;
using Optional;
using Optional.Async;

namespace Doitsu.Ecommerce.ApplicationCore.Services.BusinessServices
{
    public partial class ProductBusinessService : IProductBusinessService
    {
        public async Task<Option<int, string>> UpdateProductOptionAsync(int productId, ProductOptions data)
        {
            return await (productId, data).SomeNotNull()
                .WithException(string.Empty)
                .Filter(req => req.data != null, "Dữ liệu thuộc tính rỗng.")
                .Filter(req => req.data.Id != 0, "Id của thuộc tính này đang là 0, không thể cập nhật.")
                .Filter(req => req.productId != 0, "Id của sản phẩm này đang là 0, không thể cập nhật.")
                .FlatMapAsync(async req =>
                {
                    var product = await productRepository.FirstOrDefaultAsync(new ProductFilterByIdsSpecification(req.productId));
                    if (product == null) return Option.None<int, string>($"Không tìm thấy sản phẩm tương ứng với id {req.productId}");
                    else if (!product.ProductOptions.Any(po => po.Id == req.data.Id)) return Option.None<int, string>($"Không tìm thấy mã thuộc tính tương ứng với id {req.data.Id}");
                    else
                    {
                        var createNewValues = req.data.ProductOptionValues
                            .Where(reqDataPov => reqDataPov.Id == 0)
                            .Select(reqDataPov => reqDataPov.Value.Trim());

                        if (product.ProductOptions.Any(po => po.Id == req.data.Id && po.ProductOptionValues.Any(pov => createNewValues.Contains(pov.Value))))
                        {
                            return Option.None<int, string>($"Giá trị cho thuộc tính mà bạn muốn tạo mới đã tồn tại");
                        }
                    }

                    var updatePo = product.ProductOptions.First(po => po.Id == req.data.Id);
                    updatePo.ProductOptionValues = req.data.ProductOptionValues.Select(pov =>
                    {
                        pov.Value = pov.Value.Trim();
                        var existPov = updatePo.ProductOptionValues.FirstOrDefault(dbPov => dbPov.Id == pov.Id);
                        if (existPov != null)
                        {
                            pov.Vers = existPov.Vers;
                            pov.Active = existPov.Active;
                            pov.ProductOptionId = existPov.ProductOptionId;
                        }
                        return pov;
                    }).ToImmutableList();
                    updatePo.Name = data.Name.Trim();

                    return await UpdateProductAndRelationAsync(product);
                });
        }

        
    }
}