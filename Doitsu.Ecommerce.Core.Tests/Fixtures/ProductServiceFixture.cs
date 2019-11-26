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
        public ICollection<ProductOptionViewModel> Product1ProductOptionsData { get; }
        public ICollection<ProductOptionViewModel> Product2ProductOptionsData { get; }
        public ICollection<ProductOptionViewModel> Product3ProductOptionsData { get; }

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


            Product1ProductOptionsData = new List<ProductOptionViewModel>()
            {
                new ProductOptionViewModel()
                {
                    Name = "Nhà Mạng",
                    ProductOptionValues = new List<ProductOptionValueViewModel>()
                    {
                        new  ProductOptionValueViewModel()
                        {
                            Value = "Vinaphone"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "Viettel"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "Mobiphone"
                        }
                    }
                },
                new ProductOptionViewModel()
                {
                    Name = "Ưu tiên",
                    ProductOptionValues = new List<ProductOptionValueViewModel>()
                    {
                        new  ProductOptionValueViewModel()
                        {
                            Value = "Không ưu tiên"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "1%"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "2%"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "3%"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "4%"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "5%"
                        }
                    }
                },
                new ProductOptionViewModel()
                {
                    Name = "Loại tài khoản",
                    ProductOptionValues = new List<ProductOptionValueViewModel>()
                    {
                        new  ProductOptionValueViewModel()
                        {
                            Value = "Trả trước"
                        },
                        new  ProductOptionValueViewModel()
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
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
                ProductOptions = Product1ProductOptionsData
            };

            Product2ProductOptionsData = new List<ProductOptionViewModel>()
            {
                new ProductOptionViewModel()
                {
                    Name = "Nhà Mạng",
                    ProductOptionValues = new List<ProductOptionValueViewModel>()
                    {
                        new  ProductOptionValueViewModel()
                        {
                            Value = "Vinaphone"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "Viettel"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "Mobiphone"
                        }
                    }
                }
            };

            var product2 = new CreateProductViewModel()
            {
                Code = "PRODUCT02",
                Name = "NẠP TIỀN NHANH",
                Slug = "nap-tien-nhanh",
                Price = 0,
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
                ProductOptions = Product2ProductOptionsData
            };

            Product3ProductOptionsData = new List<ProductOptionViewModel>()
            {
                new ProductOptionViewModel()
                {
                    Name = "Mệnh giá",
                    ProductOptionValues = new List<ProductOptionValueViewModel>()
                    {
                        new  ProductOptionValueViewModel()
                        {
                            Value = "DK10"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "DK20"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "DK50"
                        },
                         new  ProductOptionValueViewModel()
                        {
                            Value = "DK100"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "DK200"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Value = "DK500"
                        }
                    }
                }
            };

            var product3 = new CreateProductViewModel()
            {
                Code = "PRODUCT03",
                Name = "NẠP GAME SMS",
                Slug = "nap-game-sms",
                Price = 0,
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
                ProductOptions = Product3ProductOptionsData
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
