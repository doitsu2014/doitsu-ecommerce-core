using System.ComponentModel.DataAnnotations;

namespace Doitsu.Ecommerce.Core
{
    public enum OrderTypeEnum
    {
        Sale = 0,
        Desposit = 1,
        Withdrawal = 2
    }

    public enum OrderPriorityEnum
    {
        OnePercent = 1,
        TwoPercent = 2,
        ThreePercent = 3,
        FourPercent = 4,
        FivePercent = 5
    }

    public enum OrderStatusEnum
    {
        New = 0,
        Processing = 1,
        Done = 2,
        Cancel = 3,
        Fail = 4
    }

    public enum UserTransactionTypeEnum
    {
        Income = 0,
        Expense = 1,
        // Roll back for a order.
        Rollback = 2,
        Withdrawal = 3
    }

    public enum UserTransactionSignEnum
    {
        Plus = '+',
        Substract = '-' 
    }


    public enum ProductOptionValueStatusEnum
    {
        Unavailable = 0,
        Available = 1
    }

    public enum ProductVariantStatusEnum
    {
        Unavailable = 0,
        Available = 1
    }

    public enum GenderEnum
    {
        [Display(Name = "Không rõ")]
        Unknown = 0, [Display(Name = "Nam")]
        Male = 1, [Display(Name = "Nữ")]
        Female = 2
    }

    public enum DayOfWeek
    {
        [Display(Name = "Thứ 2")]
        Monday = 0,
        [Display(Name = "Thứ 3")]
        Tuesday = 1,
        [Display(Name = "Thứ 4")]
        Wednesday = 2,
        [Display(Name = "Thứ 5")]
        Thursday = 3,
        [Display(Name = "Thứ 6")]
        Friday = 4,
        [Display(Name = "Thứ 7")]
        Saturday = 5,
        [Display(Name = "Chủ nhật")]
        Sunday = 6
    }

    public enum CustomerFeedBackTypeEnum
    {
        /// This is a type to he;p customer create a feedback to website. 
        Normal = 0,
        /// This is a type to help customer create a contact point to receive detail information of No Price Product
        ContactProduct = 1
    }
}