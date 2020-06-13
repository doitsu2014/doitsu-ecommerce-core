using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Doitsu.Ecommerce.Core.SEO.Helpers;
using Doitsu.Ecommerce.Core.SEO.Interfaces;
using Microsoft.Extensions.Options;
using Schema.NET;
using System;
using System.Collections.Generic;
using System.Text;

namespace Doitsu.Ecommerce.Core.SEO.Services
{
    public class WebsiteGenerator : IWebsiteGenerator
    {
        private string baseUri;
        public WebsiteGenerator(IOptions<DomainModel> domainModel)
        {
            this.baseUri = domainModel.Value.BaseUri;
        }

        public WebSite GenerateWebsite()
        {
            try
            {
                var webSite = new WebSite
                {
                    Id = new Uri(baseUri),
                    Url = new Uri(baseUri),
                    AggregateRating = new AggregateRating
                    {
                        BestRating = 5,
                        WorstRating = 1,
                        RatingCount = 100,
                        RatingValue = 5
                    }

                };
                return webSite;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Organization GenerateOrganizationFromBrand(BrandViewModel brand)
        {
            try
            {
                var organization = new Organization
                {
                    Id = new Uri(baseUri),
                    Name = brand.Name,
                    Url = new Uri(baseUri),
                    Logo = SEOHelper.ChangeToUri(new List<string>
                    {
                        brand.LogoSquareUrl,
                        brand.LogoRectangleUrl
                    }),
                    ContactPoint = new ContactPoint
                    {
                        ContactType = "customer support",
                        Telephone = brand.HotLine,
                        AvailableLanguage = new List<string>
                        {
                            "EN",
                            "VN"
                        },
                        AreaServed = "VN"
                    },
                    SameAs = SEOHelper.ChangeToUri(new List<string>
                    {
                        brand.FacebookUrl,
                        brand.GooglePlusUrl,
                        brand.TwitterUrl,
                        brand.YoutubeUrl,
                        brand.InstagramUrl,
                        brand.LinkedInUrl
                    }),
                    AggregateRating = new AggregateRating
                    {
                        BestRating = 5,
                        WorstRating = 1,
                        RatingCount = 100,
                        RatingValue = 5
                    }
                };

                return organization;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GenerateWebsiteJsonLd()
        {
            return GenerateWebsite().ToString();
        }

        public string GenerateOrganizationJsonLdFromBrand(BrandViewModel brand)
        {
            return GenerateOrganizationFromBrand(brand).ToString();
        }
    }
}
