using LafiamiAPI.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LafiamiAPI.Utilities.Enums
{
    public enum MoneyUnitEnum
    {
        [Display(Name = "Monthly"), UtilityDisplay(MountCount = 1)]
        Monthly = 0,
        [Display(Name = "Quaterly"), UtilityDisplay(MountCount = 3)]
        Quaterly,
        [Display(Name = "Bi-Annually"), UtilityDisplay(MountCount = 6)]
        BiAnnually,
        [Display(Name = "Annually"), UtilityDisplay(MountCount = 12)]
        Annually,
        [Display(Name = "One-Off"), UtilityDisplay(MountCount = 1)]
        OneOff,
    }
}
