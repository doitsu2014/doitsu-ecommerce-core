using System;
using Xunit;
using Doitsu.Ecommerce.Core.Data.Entities;
using System.Collections.Generic;
using Doitsu.Ecommerce.Core.Data;

namespace Doitsu.Ecommerce.Core.Tests.Helpers
{
    public class ProductServiceFixture : IDisposable
    {
        public string ServicePoolKey { get; set; }
        public ICollection<Categories> CategoryData { get; }
        public ICollection<Products> ProductData { get; }
        public ICollection<ProductOptions> Product1ProductOptionsData { get; }
        public ICollection<ProductOptions> Product2ProductOptionsData { get; }
        public ICollection<ProductOptions> Product3ProductOptionsData { get; }

        public ProductServiceFixture()
        {
            ServicePoolKey = "ProductService";
            CategoryData = new List<Categories>()
            {
                new Categories()
                {
                    Name = "Phone Card SMS",
                    IsFixed = false,
                    ParentCateId = null,
                    Slug = "phone-card-sms"
                }
            };

            
            Product1ProductOptionsData = new List<ProductOptions>()
            {
                new ProductOptions()
                {
                    Name = "Nhà Mạng",
                    ProductOptionValues = new List<ProductOptionValues>()
                    {
                        new ProductOptionValues()
                        {
                            Value = "Vinaphone"
                        },
                        new ProductOptionValues()
                        {
                            Value = "Viettel"
                        }
                    }
                },
                new ProductOptions()
                {
                    Name = "Ưu tiên",
                    ProductOptionValues = new List<ProductOptionValues>()
                    {
                        new ProductOptionValues()
                        {
                            Value = "1%"
                        },
                        new ProductOptionValues()
                        {
                            Value = "2%"
                        }
                    }
                },
                new ProductOptions()
                {
                    Name = "Loại tài khoản",
                    ProductOptionValues = new List<ProductOptionValues>()
                    {
                        new ProductOptionValues()
                        {
                            Value = "Trả trước"
                        },
                        new ProductOptionValues()
                        {
                            Value = "Trả sau"
                        }
                    }
                }
            };

            var product1 = new Products()
            {
                Code = "PRODUCT01",
                Name = "NẠP TRẢ TRƯỚC & TRẢ SAU",
                Slug = "nap-the-tra-truoc-va-tra-sau",
                Price = 0,
                Sku = 0,
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
                ProductOptions = Product1ProductOptionsData
            };

            var product2 = new Products()
            {
                Code = "PRODUCT02",
                Name = "NẠP TIỀN NHANH",
                Slug = "nap-tien-nhanh",
                Price = 0,
                Sku = 0,
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
            };

            var product3 = new Products()
            {
                Code = "PRODUCT03",
                Name = "NẠP GAME SMS",
                Slug = "nap-game-sms",
                Price = 0,
                Sku = 0,
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
            };

            ProductData = new List<Products>()
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
