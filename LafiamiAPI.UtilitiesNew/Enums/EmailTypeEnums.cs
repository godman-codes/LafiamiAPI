using LafiamiAPI.Utilities.Constants;
using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Utilities.Enums
{
    public enum EmailTypeEnums
    {
        [Display(Name = "New Account Template", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.NewPassword + Constants.Constants.CommaSpace + TagName.EmailAddress + Constants.Constants.CommaSpace + TagName.PhoneNumber)]
        NewAccount = 1,
        [Display(Name = "Reset Password Template", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.NewPassword + Constants.Constants.CommaSpace + TagName.EmailAddress)]
        ResetPassword,
        [Display(Name = "Change Password Template", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.NewPassword + Constants.Constants.CommaSpace + TagName.EmailAddress)]
        ChangePassword,


        [Display(Name = "New Order Template", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.OrderId + Constants.Constants.CommaSpace + TagName.Amount + Constants.Constants.CommaSpace + TagName.AmountInWord + Constants.Constants.CommaSpace + TagName.OrderPageURL)]
        NewOrder,

        [Display(Name = "New Payment Template", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.OrderId + Constants.Constants.CommaSpace + TagName.Amount + Constants.Constants.CommaSpace + TagName.TransactionId + Constants.Constants.CommaSpace + TagName.AmountInWord + Constants.Constants.CommaSpace + TagName.OrderPageURL)]
        NewPayment,

        [Display(Name = "Successful Payment Template", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.OrderId + Constants.Constants.CommaSpace + TagName.Amount + Constants.Constants.CommaSpace + TagName.TransactionId + Constants.Constants.CommaSpace + TagName.AmountInWord + Constants.Constants.CommaSpace + TagName.OrderPageURL)]
        NewSuccessfulPayment,
        [Display(Name = "Failed Payment Template", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.OrderId + Constants.Constants.CommaSpace + TagName.Amount + Constants.Constants.CommaSpace + TagName.TransactionId + Constants.Constants.CommaSpace + TagName.AmountInWord + Constants.Constants.CommaSpace + TagName.OrderPageURL)]
        NewFailedPayment,
        [Display(Name = "Pending Payment Template", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.OrderId + Constants.Constants.CommaSpace + TagName.Amount + Constants.Constants.CommaSpace + TagName.TransactionId + Constants.Constants.CommaSpace + TagName.AmountInWord + Constants.Constants.CommaSpace + TagName.OrderPageURL)]
        PendingPayment,
        [Display(Name = "Awaiting Payment Confirmation", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.OrderId + Constants.Constants.CommaSpace + TagName.Amount + Constants.Constants.CommaSpace + TagName.TransactionId + Constants.Constants.CommaSpace + TagName.AmountInWord + Constants.Constants.CommaSpace + TagName.OrderPageURL)]
        AwaitingPaymentConfirmation,


        [Display(Name = "New Wallet Transaction", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.Amount + Constants.Constants.CommaSpace + TagName.Balance + Constants.Constants.CommaSpace + TagName.BookBalance + Constants.Constants.CommaSpace + TagName.AmountInWord + Constants.Constants.CommaSpace + TagName.TransactionType)]
        NewWalletTransaction,


        [Display(Name = "Email Header", Description = Constants.Constants.ToPersonanized + TagName.WebsiteLogo + Constants.Constants.CommaSpace + TagName.WebsiteName)]
        EmailHeader,
        [Display(Name = "Email Footer", Description = Constants.Constants.ToPersonanized + TagName.WebsiteLogo + Constants.Constants.CommaSpace + TagName.WebsiteName)]
        EmailFooter,


        [Display(Name = "Background Order Integration Issue", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.Message + Constants.Constants.CommaSpace + TagName.Action + Constants.Constants.CommaSpace + TagName.CompanyName + Constants.Constants.CommaSpace + TagName.OrderPageURL)]
        OrderIntegrationIssue,
        [Display(Name = "Complete Hygeia Order Notification Template", Description = Constants.Constants.ToPersonanized + TagName.FirstName + Constants.Constants.CommaSpace + TagName.Surname + Constants.Constants.CommaSpace + TagName.OrderId + Constants.Constants.CommaSpace + TagName.Amount + Constants.Constants.CommaSpace + TagName.AmountInWord + Constants.Constants.CommaSpace + TagName.OrderPageURL + Constants.Constants.CommaSpace + TagName.EnrolleeNumber)]
        CompleteHygeiaOrderNotification,

    }
}
