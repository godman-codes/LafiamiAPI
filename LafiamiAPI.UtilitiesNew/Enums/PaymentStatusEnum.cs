using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Utilities.Enums
{
    public enum PaymentStatusEnum
    {
        Pending = 1,
        Paid,
        Failed,
        [Display(Name = "Inconsistency Amount")]
        InconsistencyAmount,
        Cancelled,
        [Display(Name = "Awaiting Verification")]
        AwaitingVerification
    }
}
