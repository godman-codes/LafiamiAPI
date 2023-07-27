using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Utilities.Enums
{
    public enum ShippingStatusEnum
    {
        Pending = 1,
        Processing,
        Cancelled,
        [Display(Name = "On Route")]
        OnRoute,
        Delivered,
        [Display(Name = "Pickup By Customer")]
        PickedUp
    }
}
