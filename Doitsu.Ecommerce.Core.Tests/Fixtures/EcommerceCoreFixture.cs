using System;
using Xunit;
using Doitsu.Ecommerce.Core.Data.Entities;
using System.Collections.Generic;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.ViewModels;

namespace Doitsu.Ecommerce.Core.Tests.Helpers
{
    public class EcommerceCoreFixture : IDisposable
    {
        public BrandViewModel BrandData { get; }
        public ICollection<CategoryWithInverseParentViewModel> CategoryData { get; }
        public ICollection<CreateProductViewModel> ProductData { get; }
        public ICollection<ProductOptionViewModel> Product1ProductOptionsData { get; }

        public EcommerceCoreFixture()
        {
            BrandData = new BrandViewModel()
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

            CategoryData = new List<CategoryWithInverseParentViewModel>()
            {
                new CategoryWithInverseParentViewModel()
                {
                    Id = 0,
                    Name = "Sản phẩm",
                    IsFixed = true,
                    ParentCateId = null,
                    Slug = "san-pham",
                    InverseParentCate = new List<CategoryWithInverseParentViewModel>()
                    {
                        new CategoryWithInverseParentViewModel()
                        {
                            Name = "Hàng bán",
                            IsFixed = true,
                            ParentCateId = null,
                            Slug = "hang-ban"
                        }
                    }
                }
            };

            Product1ProductOptionsData = new List<ProductOptionViewModel>()
            {
                new ProductOptionViewModel()
                {
                    Id = 0,
                    Name = "Nhà Mạng",
                    ProductOptionValues = new List<ProductOptionValueViewModel>()
                    {
                        new  ProductOptionValueViewModel()
                        {
                            Id = 0,
                            Value = "Vinaphone"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Id = 0,
                            Value = "Viettel"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Id = 0,
                            Value = "Mobiphone"
                        }
                    }
                },
                new ProductOptionViewModel()
                {
                    Id = 0,
                    Name = "Loại tài khoản",
                    ProductOptionValues = new List<ProductOptionValueViewModel>()
                    {
                        new  ProductOptionValueViewModel()
                        {
                            Id = 0,
                            Value = "Trả trước"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Id = 0,
                            Value = "Trả sau"
                        }
                    }
                },
                new ProductOptionViewModel()
                {
                    Id = 0,
                    Name = "Có mật khẩu ứng dụng",
                    ProductOptionValues = new List<ProductOptionValueViewModel>()
                    {
                        new  ProductOptionValueViewModel()
                        {
                            Id = 0,
                            Value = "Có"
                        },
                        new  ProductOptionValueViewModel()
                        {
                            Id = 0,
                            Value = "Không"
                        }
                    }
                }
            };

            var product1 = new CreateProductViewModel()
            {
                Id = 0,
                Code = "GACH-CUOC",
                Name = "NẠP TRẢ TRƯỚC & TRẢ SAU",
                Slug = "nap-the-tra-truoc-va-tra-sau",
                Price = 0,
                Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Quas, beatae dolorem, Quas, beatae dolorem, Quas, beatae dolorem aperiam distinctio ex facere, eos recusandae quod non inventore sint debitis aspernatur similique! Molestiae dicta odio cupiditate quia iusto?",
                Weight = 1000,
                ProductOptions = Product1ProductOptionsData
            };

            ProductData = new List<CreateProductViewModel>()
            {
                product1
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
