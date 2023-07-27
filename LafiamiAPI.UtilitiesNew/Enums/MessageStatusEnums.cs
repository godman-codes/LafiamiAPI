using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Utilities.Enums
{
    public enum MessageStatusEnums
    {
        Pending,
        Sent,
        Failed,
        [Display(Name = "Sent Partially")]
        SentPartially
    }
}
