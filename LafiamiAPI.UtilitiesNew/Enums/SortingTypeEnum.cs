using LafiamiAPI.Utilities.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Utilities.Enums
{
    public enum SortingTypeEnum
    {
        [Display(Name = "Latest"), UtilityDisplay(IsPrivate = false)]
        Latest,
        [Display(Name = "Oldest"), UtilityDisplay(IsPrivate = false)]
        Oldest,
        [Display(Name = "Lowest Price"), UtilityDisplay(IsPrivate = false)]
        LowestPrice,
        [Display(Name = "Highest Price"), UtilityDisplay(IsPrivate = false)]
        HighestPrice,
        [Display(Name = "Popular Insurance Plans"), UtilityDisplay(IsPrivate = false)]
        Popular,
        [Display(Name = "Random"), UtilityDisplay(IsPrivate = true)]
        Random
    }
}
