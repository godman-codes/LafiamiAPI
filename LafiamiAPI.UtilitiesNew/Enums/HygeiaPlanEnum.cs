using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LafiamiAPI.Utilities.Enums
{
    public enum HygeiaPlanEnum
    {
        [Display(Name = "Lafiami Flex Plan")]
        LafiamiBasicPlan = 1637,
        [Display(Name = "Lafiami Flenjo Plan")]
        LafiamiMidiPlan = 1638,
        [Display(Name = "Lafiami Jolly Plan")]
        LafiamiIntermediaryPlan = 1639
    }
}
