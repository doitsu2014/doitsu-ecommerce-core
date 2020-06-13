using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Doitsu.Ecommerce.Core.SEO.Helpers;
using Doitsu.Ecommerce.Core.SEO.Interfaces;
using Microsoft.Extensions.Options;
using Schema.NET;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Doitsu.Ecommerce.Core.SEO.Services
{
    public class ProductGenerator : IProductGenerator
    {
        private string baseUri;
        public ProductGenerator(IOptions<DomainModel> domainModel)
        {
            this.baseUri = domainModel.Value.BaseUri;
        }

        public List<Product> GenerateProductFromProductDetail(ProductDetailViewModel product, Organization organization)
        {
            try
            {
                var result = new List<Product>();
                var uriStr = baseUri + "/" + product.Cate.ParentCate.Slug + "/" + product.Cate.Slug + "/" + product.Slug;
                var lowPrice = product.ProductVariants?.Where(pv => pv.AnotherPrice != 0)?.Select(pv => pv.AnotherPrice)?.Min();
                var highPrice = product.ProductVariants?.Where(pv => pv.AnotherPrice != 0)?.Select(pv => pv.AnotherPrice)?.Max();
                var cate = product.Cate.ParentCate.Name + " > " + product.Cate.Name;

                var parentProperties = product.ProductOptions.Select(po =>
                {
                    IPropertyValue pv = new PropertyValue
                    {
                        Name = po.Name,
                        Value = po.ProductOptionValues.Select(pov => pov.Value).ToList()
                    };
                    return pv;
                }).ToList();

                var parentProduct = new Product
                {
                    Id = new Uri(uriStr),
                    Name = product.Name,
                    Brand = new Brand
                    {
                        Name = "No brand"
                    },
                    Sku = product.ProductVariants.Select(pv => pv.Sku).ToList(),
                    Description = new List<string>
                    {
                        product.Description,
                        product.ShortDescription
                    },
                    Url = new Uri(uriStr),
                    ProductID = product.Code,
                    Image = SEOHelper.ChangeToUri(new List<string>
                    {
                        baseUri + "/" + product.ImageThumbUrl,
                        string.IsNullOrEmpty(product.ImageUrls) ? null :  baseUri + "/" + product.ImageUrls
                    }),
                    Offers = new AggregateOffer
                    {
                        Seller = organization,
                        Price = product.Price,
                        PriceCurrency = "VND",
                        Availability = ItemAvailability.InStock,
                        ItemCondition = OfferItemCondition.NewCondition,
                        Url = new Uri(uriStr),
                        LowPrice = lowPrice < product.Price ? lowPrice : product.Price,
                        HighPrice = highPrice > product.Price ? highPrice : product.Price,
                        OfferCount = product.ProductVariants.Count + 1
                    },
                    AdditionalProperty = parentProperties,
                    Category = cate,
                    AggregateRating = new AggregateRating
                    {
                        BestRating = 5,
                        WorstRating = 1,
                        RatingCount = 90,
                        RatingValue = 5
                    },
                    Review = new List<IReview>()
                    {
                        new Review
                        {
                            DatePublished = DateTime.Parse("05-12-2020"),
                            Author = new Person
                            {
                                Name = "quinniechan"
                            },
                            ReviewRating = new Rating
                            {
                                RatingValue = 4.8
                            },
                            ReviewBody = "Hơi có 1 xíu trục trặc nhưng nhìn chung vẫn siêu cưng. Còn được tặng kèm khẩu trang YGFL nữa <3 "
                        },
                        new Review
                        {
                            DatePublished = DateTime.Parse("12-10-2020"),
                            Author = new Person
                            {
                                Name = "love_kpop55"
                            },
                            ReviewRating = new Rating
                            {
                                RatingValue = 5
                            },
                            ReviewBody = "Đóng gói siêu chắc chắn, giao đúng hạn, còn được shop tặng khẩu trang nữa"
                        },
                        new Review
                        {
                            DatePublished = DateTime.Parse("04-05-2020"),
                            Author = new Person
                            {
                                Name = "ginaluong"
                            },
                            ReviewRating = new Rating
                            {
                                RatingValue = 5
                            },
                            ReviewBody = "Chất lượng sản phẩm tuyệt vời"
                        },
                    }
                };
                result.Add(parentProduct);

                foreach (var option in product.ProductOptions)
                {
                    foreach (var optionValue in option.ProductOptionValues)
                    {
                        var variantProduct = product.ProductVariants.FirstOrDefault(pv => pv.Id == optionValue.Id);

                        var childProduct = new Product
                        {
                            Id = new Uri(uriStr),
                            Name = product.Name,
                            Brand = new Brand 
                            { 
                                Name = "No brand"
                            },
                            Description = new List<string>
                            {
                                product.Description,
                                product.ShortDescription
                            },
                            Url = new Uri(uriStr),
                            ProductID = product.Code,
                            Image = SEOHelper.ChangeToUri(new List<string>
                            {
                                baseUri + "/" + product.ImageThumbUrl,
                                string.IsNullOrEmpty(product.ImageUrls) ? null :  baseUri + "/" + product.ImageUrls
                            }),
                            Offers = new AggregateOffer
                            {
                                Seller = organization,
                                Price = variantProduct.ProductPrice,
                                PriceCurrency = "VND",
                                Availability = ItemAvailability.InStock,
                                ItemCondition = OfferItemCondition.NewCondition,
                                Url = new Uri(uriStr),
                                LowPrice = variantProduct.AnotherPrice == 0 ? variantProduct.ProductPrice
                                        : variantProduct.AnotherPrice < variantProduct.ProductPrice ? variantProduct.AnotherPrice
                                        : variantProduct.ProductPrice,
                                HighPrice = variantProduct.AnotherPrice == 0 ? variantProduct.ProductPrice
                                        : variantProduct.AnotherPrice > variantProduct.ProductPrice ? variantProduct.AnotherPrice
                                        : variantProduct.ProductPrice,
                                OfferCount = variantProduct.ProductPrice == 0 ? 1 : 2
                            },
                            Sku = variantProduct.Sku,
                            AdditionalProperty = new PropertyValue
                            {
                                Name = option.Name,
                                Value = variantProduct.ProductVariantOptionValues?.Select(pvo => pvo.ProductOptionValue).FirstOrDefault(pov => pov.ProductOptionId == option.Id)?.Value
                            },
                            Category = cate,
                            AggregateRating = new AggregateRating
                            {
                                BestRating = 5,
                                WorstRating = 1,
                                RatingCount = 40,
                                RatingValue = 5
                            },
                            Review = new List<IReview>()
                            {
                                new Review
                                {
                                    DatePublished = DateTime.Parse("05-12-2020"),
                                    Author = new Person
                                    {
                                        Name = "quinniechan"
                                    },
                                    ReviewRating = new Rating
                                    {
                                        RatingValue = 4.8
                                    },
                                    ReviewBody = "Hơi có 1 xíu trục trặc nhưng nhìn chung vẫn siêu cưng. Còn được tặng kèm khẩu trang YGFL nữa <3 "
                                },
                                new Review
                                {
                                    DatePublished = DateTime.Parse("12-10-2020"),
                                    Author = new Person
                                    {
                                        Name = "love_kpop55"
                                    },
                                    ReviewRating = new Rating
                                    {
                                        RatingValue = 5
                                    },
                                    ReviewBody = "Đóng gói siêu chắc chắn, giao đúng hạn, còn được shop tặng khẩu trang nữa"
                                },
                                new Review
                                {
                                    DatePublished = DateTime.Parse("04-05-2020"),
                                    Author = new Person
                                    {
                                        Name = "ginaluong"
                                    },
                                    ReviewRating = new Rating
                                    {
                                        RatingValue = 5
                                    },
                                    ReviewBody = "Chất lượng sản phẩm tuyệt vời"
                                },
                                    }
                                };
                                result.Add(childProduct);
                            }
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GenerateProductJsonLdFromProductDetail(ProductDetailViewModel productDetail, Organization organization)
        {
            return GenerateProductFromProductDetail(productDetail, organization).ToString();
        }
    }
}