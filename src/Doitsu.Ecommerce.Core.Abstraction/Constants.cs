namespace Doitsu.Ecommerce.Core.Abstraction
{
    public class Constants
    {
        public static class ClaimTypeConstants
        {
            public const string USER_ID = "UserId";
            public const string FULL_NAME = "Fullname";
        }

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
            public const string ARTIST = "nghe-si";
            public const string CATEGORY = "danh-muc";
            public const string EVENT = "su-kien";
            public const string GALLERY = "thu-vien";
        }

        public static class SuperFixedBlogCategorySlug
        {
            public const string PROMOTION = "khuyen-mai";
            public const string NEWS = "tin-tuc";
            public const string WHITE_BOARD = "thong-bao";
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
            public const string PROMOTION_BLOGS = "PROMOTION_BLOGS";
            public const string INVERSE_CATEGORY = "INVERSE_CATEGORY";
        }

        public static class UserRoles
        {
            public const string ADMIN = "Administrator";
            public const string ACTIVE_USER = "ActiveUser";
            public const string ALL = "Administrator,ActiveUser";
        }

        public static class DoitsuAuthenticationSchemes
        {
            public const string JWT_SCHEME = "api_jwt";
        }

        public static class OrderInformation
        {
            public const int ORDER_CODE_LENGTH = 12;
        }

        public static class FileExtension
        {
            public const string EXCEL = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }

        public static class DateTimeFormat
        {
            public const string Default = "MM/dd/yyyy";
        }

        public static class RoleName
        {
            public const string ACTIVE_USER = "ActiveUser";
        }

        public static class UserMessage
        {
            //Required 
            public const string EMAIL_REQUIRED = "Địa chỉ email không được bỏ trống.";
            public const string PASSWORD_REQUIRED = "Mật khẩu không được bỏ trống.";
            public const string NEW_PASSWORD_REQUIRED = "Mật khẩu xác nhận không được để trống.";
            public const string PHONE_REQUIRED = "Số điện thoại không được bỏ trống.";
            public const string FULLNAME_REQUIRED = "Tên không được bỏ trống.";
            public const string REQUEST_REQUIRED = "Dữ liệu truyền lên rỗng.";

            //Regex 
            public const string PASSWORD_REGEX_FAIL = "Mật khẩu của bạn phải chứa ít nhất 1 ký tự đặc biệt và 1 ký tự viết hoa.";
            public const string CONFIRM_PASSWORD_NOT_MATCH = "Mật khẩu và mật khẩu xác nhận không trùng khớp.";

            //Existed
            public const string EMAIL_EXISTED = "Đã tồn tại địa chỉ email {0} trong hệ thống.";
            public const string PHONE_EXISTED = "Đã tồn tại số điện thoại {0} trong hệ thống.";

            //Not existed
            public const string PHONE_EMAIL_NOT_EXISTED = "Không tìm thấy người dùng với email hay số điện thoại: {0}.";
            public const string USER_NOT_EXISTED = "Tài khoản người dùng này không tồn tại!";

            //Login
            public const string LOGIN_NULL_REQUEST = "Thông tin đăng nhập rỗng.";
            public const string LOGIN_FAILED = "Thông tin đăng nhập không hợp lệ.";
            public const string LOGIN_SUCCESS = "Đăng nhập thành công.";

            //Register
            public const string REGISTER_NULL_REQUEST = "Dữ liệu nhận vào không hợp lệ.";
            public const string REGISTER_AUTH_FAILED = "Phân quyền người dùng thất bại.";
            public const string REGISTER_SUCCESS = "Đăng ký thành công.";
            public const string CHECKOUT_CARD_NULL = "Giỏ hàng rỗng, không thể tạo đơn hàng cho bạn.";
            public const string CREATE_NEW_FAILED = "Tạo người dùng {0} thât bại!\n có thể do trùng tên đăng nhập.";

            //Reset password 
            public const string RESET_PASSWORD_NULL_REQUEST = "Dữ liệu để đặt lại mật khẩu rỗng, xin vui lòng thử lại.";
            public const string RESET_PASSWORD_IDENTIFICATION_NULL = "Thông tin định danh để đặt lại mật khẩu bị lỗi. Xin vui lòng gửi lại yêu cầu.";
            public const string RESET_PASSWORD_TOKEN_NULL = "Mã định danh để đặt lại mật khẩu bị lỗi. Xin vui lòng gửi lại yêu cầu.";
            public const string RESET_PASSWORD_SUCCESS = "Đặt lại mật khẩu thành công.";
            public const string RESET_PASSWORD_FAIL = "Đặt lại mật khẩu không thành công.";

            //Change detail
            public const string CHANGE_DETAIL_NULL_REQUEST = "Dữ liệu để thay đổi thông tin rỗng, xin vui lòng thử lại.";
            public const string CHANGE_DETAIL_SUCCESS = "Thay đổi thông tin người dùng thành công.";
            public const string CHANGE_DETAIL_FAIL = "Thay đổi thông tin người dùng thất bại.";

            //Update 
            public const string UPDATE_USER_FAIL = "Cập nhật người dùng {0} thât bại!";

            //Delivery
            public const string DELIVERY_NOT_EXISTED = "Không tìm thấy thông tin địa chỉ.";
            public const string DELIVERY_EXISTED = "Địa chỉ này đã tồn tại.";
        }

        public static class SendEmailProperty
        {
            //Reset password
            public const string RESET_PASSWORD_SUBJECT = "[{0}] Xác nhận yêu cầu đặt lại mật khẩu";
            public const string RESET_PASSWORD_BODY = "<p>Hãy nhập vào link này để <a href='{0}://{1}/nguoi-dung/dat-lai-mat-khau?userIdentification={2}&token={3}'>đặt lại mật khẩu</a></p>";
            public const string RESET_PASSWORD_REQUEST_SUCCESS = "Gửi yêu cầu đặt lại mật khẩu thành công.";
        }

        public static class ModelErrorKey
        {
            public const string SERVER_ERROR = "SERVER_ERROR";
        }
    }
}