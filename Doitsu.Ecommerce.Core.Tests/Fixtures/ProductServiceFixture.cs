using System;
using Xunit;
using Doitsu.Ecommerce.Core.Data.Entities;
using System.Collections.Generic;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.ViewModels;

namespace Doitsu.Ecommerce.Core.Tests.Helpers
{
    public class ProductServiceFixture : IDisposable
    {
        public string ServicePoolKey { get; set; }
        public ICollection<CategoryViewModel> CategoryData { get; }
        public ICollection<CreateProductViewModel> ProductData { get; }
        public ICollection<BaseEditProductOptionViewModel> Product1ProductOptionsData { get; }
        public ICollection<BaseEditProductOptionViewModel> Product2ProductOptionsData { get; }
        public ICollection<BaseEditProductOptionViewModel> Product3ProductOptionsData { get; }

        public ProductServiceFixture()
        {
            ServicePoolKey = "ProductService";
            CategoryData = new List<CategoryViewModel>()
            {
                new CategoryViewModel()
                {
                    Name = "Phone Card SMS",
                    IsFixed = false,
                    ParentCateId = null,
                    Slug = "phone-card-sms"
                }
            };

            
            Product1ProductOptionsData = new List<BaseEditProductOptionViewModel>()
            {
                new BaseEditProductOptionViewModel()
                {
                    Name = "Nhà Mạng",
                    ProductOptionValues = new List<BaseEditProductOptionValueViewModel>()
                    {
                        new BaseEditProductOptionValueViewModel()
                        {
                            Value = "Vinaphone"
                        },
                        new BaseEditProductOptionValueViewModel()
                        {
                            Value = "Viettel"
                        }
                    }
                },
                new BaseEditProductOptionViewModel()
                {
                    Name = "Ưu tiên",
                    ProductOptionValues = new List<BaseEditProductOptionValueViewModel>()
                    {
                        new BaseEditProductOptionValueViewModel()
                        {
                            Value = "1%"
                        },
                        new BaseEditProductOptionValueViewModel()
                        {
                            Value = "2%"
                        }
                    }
                },
                new BaseEditProductOptionViewModel()
                {
                    Name = "Loại tài khoản",
                    ProductOptionValues = new List<BaseEditProductOptionValueViewModel>()
                    {
                        new BaseEditProductOptionValueViewModel()
                        {
                            Value = "Trả trước"
                        },
                        new BaseEditProductOptionValueViewModel()
                        {
                            Value = "Trả sau"
                        }
                    }
                }
            };

            var product1 = new CreateProductViewModel()
            {
                Code = "PRODUCT01",
                Name = "NẠP TRẢ TRƯỚC & TRẢ SAU",
                Slug = "nap-the-tra-truoc-va-tra-sau",
                Price = 0,
                Sku = 0,
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
                ProductOptions = Product1ProductOptionsData
            };

            var product2 = new CreateProductViewModel()
            {
                Code = "PRODUCT02",
                Name = "NẠP TIỀN NHANH",
                Slug = "nap-tien-nhanh",
                Price = 0,
                Sku = 0,
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
            };

            var product3 = new CreateProductViewModel()
            {
                Code = "PRODUCT03",
                Name = "NẠP GAME SMS",
                Slug = "nap-game-sms",
                Price = 0,
                Sku = 0,
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
            };

            ProductData = new List<CreateProductViewModel>()
            {
                product1, product2, product3
            };

        
        }

        public void Dispose()
        {
            // Do Nothing
        }
    }


    [CollectionDefinition("ProductServiceCollections")]
    public class ProductServiceCollections : ICollectionFixture<ProductServiceFixture>
    {
    }

}
