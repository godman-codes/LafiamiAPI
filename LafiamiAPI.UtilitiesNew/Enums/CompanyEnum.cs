using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LafiamiAPI.Utilities.Enums
{
    public enum CompanyEnum
    {
        [Display(Name = "Lafiami")]
        Lafiami = 1,
        [Display(Name = "Aiico")]
        Aiico,
        [Display(Name = "Hygeia")]
        Hygeia,
        [Display(Name = "Axa Mansard")]
        AxaMansand,
        [Display(Name = "Relaince HMO")]
        RelainceHMO,
    }
}
