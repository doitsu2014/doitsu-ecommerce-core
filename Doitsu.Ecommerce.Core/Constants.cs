namespace Doitsu.Ecommerce.Core
{
    public class Constants
    {
        public static class BrandInformation
        {
            public const int Id = 1;
        }

        /// <summary>
        /// This constant is a mapping meta data of slug value on Super Fixed Cateogories Data Range.
        /// </summary>
        public static class SuperFixedCategorySlug
        {
            public const string PRODUCT = "san-pham";
            public const string BUILDING = "cong-trinh";
        }

        public static class CacheKey
        {
            public const string MENU_ITEMS = "MENU_ITEMS_CACHE";
            public const string BUILDING_CATEGORY_CHILDREN = "BUILDING_CATEGORY_CHILDREN";
            public const string RANDOM_PRODUCTS = "RANDOM_PRODUCTS";
            public const string BRAND_INFORMATION = "BRAND_INFORMATION";
            public const string TOP_TAGS = "TOP_TAGS";
            public const string RANDOM_BLOGS = "RANDOM_BLOGS";
            public const string SLIDERS = "SLIDERS";
            public const string CATALOGUES = "CATALOGUES";
        }

        public static class UserRoles
        {
            public const string ADMIN = "Administrator";
            public const string ACTIVE_USER = "ActiveUser";
            public const string ALL = "Administrator,ActiveUser";
        }
    }
}