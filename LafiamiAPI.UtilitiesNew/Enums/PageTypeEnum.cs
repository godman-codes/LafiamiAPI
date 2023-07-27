using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LafiamiAPI.Utilities.Enums
{
    public enum PageTypeEnum
    {
        Home, 
        About,
        Contact,
        [Display(Name = "Find a Plan")]
        FindAPlan,
        [Display(Name = "Compare Plans")]
        ComparePlans,
        Products,
        Plan,
        [Display(Name = "Unknown Page")]
        UnknownPage,
        [Display(Name = "Check Out")]
        CheckOut,
        Order,
        Login,
        Register,
        Privacy,
        [Display(Name = "Term Of Use")]
        TermOfUse,
        Intellectual,
        Disclaimer
    }
}
