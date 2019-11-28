using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class UserInforViewModel
    {
        [JsonProperty("fullName")]
        public string Fullname
        {
            get;
            set;
        }

        [JsonProperty("phoneNumer")]
        public string PhoneNumber
        {
            get;
            set;
        }

        [JsonProperty("email")]
        public string Email
        {
            get;
            set;
        }

        [JsonProperty("address")]
        public string Address
        {
            get;
            set;
        }

        [JsonProperty("gender")]
        public GenderEnum Gender
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Không được bỏ trống")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại", Prompt = "Nhập mật khẩu hiện tại của bạn")]
        [JsonProperty("currentPassword")]
        public string CurrentPassword
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Không được bỏ trống")]
        [DataType(DataType.Password)]
        [MaxLength(32, ErrorMessage = "Mật khẩu vượt quá 32 ký tự")]
        [Display(Name = "Mật khẩu mới", Prompt = "Nhập mật khẩu của bạn tối đa 32 ký tự")]
        [JsonProperty("newPassword")]
        public string NewPassword
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Không được bỏ trống")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không trùng khớp")]
        [Display(Name = "Xác nhận mật khẩu mới", Prompt = "Nhập lại mật khẩu")]
        [JsonProperty("newPasswordConfirm")]
        public string NewPasswordConfirm
        {
            get;
            set;
        }
    }

    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Không được bỏ trống")]
        [DataType(DataType.EmailAddress)]
        public string Email
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Không được bỏ trống")]
        [DataType(DataType.Password)]
        [MaxLength(32, ErrorMessage = "Mật khẩu vượt quá 32 ký tự")]
        [Display(Name = "Mật khẩu", Prompt = "Nhập mật khẩu")]
        public string Password
        {
            get;
            set;
        }
    }

    public class LoginViewModel
    {


        [Required(ErrorMessage = "Không được bỏ trống")]
        [DataType(DataType.Password)]
        [MaxLength(32, ErrorMessage = "Mật khẩu vượt quá 32 ký tự")]
        [Display(Name = "Mật khẩu", Prompt = "Nhập mật khẩu")]
        public string Password
        {
            get;
            set;
        }
    }

    public class LoginByPhoneViewModel : LoginViewModel
    {
        [Required(ErrorMessage = "Không được bỏ trống")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Số điện thoại không đúng mẫu")]
        [Display(Name = "Số điện thoại", Prompt = "Nhập vào số điện thoại")]
        public string PhoneNumber
        {
            get;
            set;
        }
    }

    public class LoginByEmailViewModel : LoginViewModel
    {
        public string Email
        {
            get;
            set;
        }
    }

    public class RegisterViewModel
    {
        [JsonProperty("phoneNumber")]
        [Required(ErrorMessage = "Không được bỏ trống")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Số điện thoại không đúng mẫu")]
        [Remote(action: "VerifyPhone", controller: "User")]
        [Display(Name = "Số điện thoại", Prompt = "Nhập vào số điện thoại")]
        public string PhoneNumber
        {
            get;
            set;
        }

        [JsonProperty("email")]
        [Required(ErrorMessage = "Không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng mẫu")]
        [Remote(action: "VerifyMail", controller: "User")]
        [Display(Name = "Email", Prompt = "Nhập vào địa chỉ email")]
        public string Email
        {
            get;
            set;
        }

        [JsonProperty("fullName")]
        [Required(ErrorMessage = "Không được bỏ trống")]
        [DataType(DataType.Text)]
        [Display(Name = "Họ và tên", Prompt = "Nhập vào họ và tên")]
        public string Fullname
        {
            get;
            set;
        }

        [JsonProperty("password")]
        [Required(ErrorMessage = "Không được bỏ trống")]
        [DataType(DataType.Password)]
        [MaxLength(32, ErrorMessage = "Mật khẩu vượt quá 32 ký tự")]
        [Display(Name = "Mật khẩu", Prompt = "Nhập mật khẩu của bạn tối đa 32 ký tự")]
        public string Password
        {
            get;
            set;
        }

        [JsonProperty("confirmPassword")]
        [Required(ErrorMessage = "Không được bỏ trống")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu và mật khẩu xác nhận không trùng khớp")]
        [Display(Name = "Xác nhận mật khẩu", Prompt = "Nhập lại mật khẩu")]
        public string ConfirmPassword
        {
            get;
            set;
        }

        [JsonProperty("address")]
        [Required(ErrorMessage = "Không được bỏ trống")]
        [MaxLength(350, ErrorMessage = "Địa chỉ vượt quá 350 ký tự")]
        [Display(Name = "Địa chỉ", Prompt = "Nhập địa chỉ")]
        public string Address
        {
            get;
            set;
        }

        [JsonProperty("gender")]
        [Required(ErrorMessage = "Bạn phải chọn giới tính")]
        [Display(Name = "Giới tính")]
        public int Gender
        {
            get;
            set;
        }

        [JsonProperty("cartInformation")]
        public CheckoutCartViewModel CartInformation
        {
            get;
            set;
        }
    }

    public class AdminEditNewUserViewModel
    {
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public List<string> RoleIds { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [JsonProperty("userIdentification")]
        public string UserIdentification
        {
            get;
            set;
        }

        [JsonProperty("newPassword")]
        public string NewPassword
        {
            get;
            set;
        }

        [JsonProperty("resetPasswordToken")]
        public string ResetPasswordToken
        {
            get;
            set;
        }
    }
}