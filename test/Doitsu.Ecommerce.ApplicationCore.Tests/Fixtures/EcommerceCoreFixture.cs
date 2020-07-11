using System;
using Xunit;
using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Models.ViewModels;
using Doitsu.Ecommerce.ApplicationCore.Entities;

namespace Doitsu.Ecommerce.Core.Tests.Helpers
{
    public class EcommerceCoreFixture : IDisposable
    {
        public Brand BrandData { get; }
        public ICollection<Categories> CategoryData { get; }
        public ICollection<Products> ProductData { get; }
        public ICollection<ProductOptions> Product1ProductOptionsData { get; }

        public EcommerceCoreFixture()
        {
            BrandData = new Brand()
            {
                Id = 0,
                Address = "Kim chỉ nam",
                AlternativeAddress = "Kim chỉ nam khác",
                Description = "Nội dung",
                Fax = "0946680600",
                HotLine = "0946680600",
                Mail = "duc.tran@doitsu.tech",
                LogoRectangleUrl = "",
                LogoSquareUrl = "",
                Name = "Phone Card",
                OpenTime = TimeSpan.FromHours(10),
                CloseTime = TimeSpan.FromHours(10),
                OpenDayOfWeek = (int)DayOfWeek.Monday,
                CloseDayOfWeek = (int)DayOfWeek.Sunday
            };

            CategoryData = new List<Categories>()
            {
                new Categories()
                {
                    Id = 0,
                    Name = "Sản phẩm",
                    IsFixed = true,
                    ParentCateId = null,
                    Slug = "san-pham",
                    InverseParentCate = new List<Categories>()
                    {
                        new Categories()
                        {
                            Name = "Hàng bán 1",
                            IsFixed = false,
                            ParentCateId = null,
                            Slug = "hang-ban-1"
                        },
                        new Categories()
                        {
                            Name = "Hàng bán 2",
                            IsFixed = false,
                            ParentCateId = null,
                            Slug = "hang-ban-2"
                        },
                        new Categories()
                        {
                            Name = "Hàng bán 3",
                            IsFixed = false,
                            ParentCateId = null,
                            Slug = "hang-ban-3"
                        }
                    }
                }
            };

            Product1ProductOptionsData = new List<ProductOptions>()
            {
                new ProductOptions()
                {
                    Id = 0,
                    Name = "Nhà Mạng",
                    ProductOptionValues = new List<ProductOptionValues>()
                    {
                        new  ProductOptionValues()
                        {
                            Id = 0,
                            Value = "Vinaphone"
                        },
                        new  ProductOptionValues()
                        {
                            Id = 0,
                            Value = "Viettel"
                        },
                        new  ProductOptionValues()
                        {
                            Id = 0,
                            Value = "Mobiphone"
                        }
                    }
                },
                new ProductOptions()
                {
                    Id = 0,
                    Name = "Loại tài khoản",
                    ProductOptionValues = new List<ProductOptionValues>()
                    {
                        new  ProductOptionValues()
                        {
                            Id = 0,
                            Value = "Trả trước"
                        },
                        new  ProductOptionValues()
                        {
                            Id = 0,
                            Value = "Trả sau"
                        }
                    }
                },
                new ProductOptions()
                {
                    Id = 0,
                    Name = "Có mật khẩu ứng dụng",
                    ProductOptionValues = new List<ProductOptionValues>()
                    {
                        new  ProductOptionValues()
                        {
                            Id = 0,
                            Value = "Có"
                        },
                        new  ProductOptionValues()
                        {
                            Id = 0,
                            Value = "Không"
                        }
                    }
                }
            };

            var product1 = new Products()
            {
                Id = 0,
                Code = "PV934581023901",
                Name = "Sản phẩm ảo 01",
                Slug = "san-pham-ao-01",
                Price = 0,
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
                ShortDescription = "Lorem ipsum dolor sit amet consectetur adipisicing elit",
                Weight = 1000,
                ProductOptions = Product1ProductOptionsData
            };

            var product2 = new Products()
            {
                Id = 0,
                Code = "PV934581028401",
                Name = "Sản phẩm ảo 02",
                Slug = "san-pham-ao-02",
                Price = 0,
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
                ShortDescription = "Lorem ipsum dolor sit amet consectetur adipisicing elit",
                Weight = 1000,
                ProductOptions = Product1ProductOptionsData
            };

            var product3 = new Products()
            {
                Id = 0,
                Code = "PV934581022817",
                Name = "Sản phẩm ảo 03",
                Slug = "san-pham-ao-03",
                Price = 0,
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
                ShortDescription = "Lorem ipsum dolor sit amet consectetur adipisicing elit",
                Weight = 1000,
                ProductOptions = Product1ProductOptionsData
            };

            ProductData = new List<Products>()
            {
                product1,
                product2,
                product3
            };
        }

        public void Dispose()
        {
            // Do Nothing
        }
    }

    [CollectionDefinition("EcommerceCoreCollection")]
    public class EcommerceCoreCollection : ICollectionFixture<EcommerceCoreFixture>
    {
    }

}
