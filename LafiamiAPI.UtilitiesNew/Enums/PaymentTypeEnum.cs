using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Utilities.Enums
{
    public enum PaymentTypeEnum
    {
        [Display(Name = "Online Payment")]
        OnlinePayment = 1,
        [Display(Name = "Cash Payment")]
        CashPayment,
    }
}
