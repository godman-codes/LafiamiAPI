using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Utilities.Enums
{
    public enum KnownSMTPEnum
    {
        [Display(Name = "Zoho(TLS)")]
        Zoho = 1,
        [Display(Name = "Zoho(SSL)")]
        Zoho_SSL = 2,
        [Display(Name = "Gmail(TLS)")]
        Gmail = 3,
        [Display(Name = "Gmail(SSL)")]
        Gmail_SSL = 4,
        [Display(Name = "Yahoo(TLS)")]
        Yahoo = 5,
        [Display(Name = "Yahoo(SSL)")]
        Yahoo_SSL = 6,
        [Display(Name = "Outlook(TLS)")]
        Outlook = 7,
        [Display(Name = "HotMail(TLS)")]
        HotMail = 8,
        [Display(Name = "Office365(TLS)")]
        Office365 = 9,
        Others = 10
    }
}
