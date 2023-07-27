using System;

namespace LafiamiAPI.Utilities.Attributes
{
    public sealed class UtilityDisplayAttribute : Attribute
    {
        public int DayCount { get; set; }
        public int MountCount { get; set; }
        public bool RequireConfirmation { get; set; }
        public bool ForNumbersOnly { get; set; }
        public string TagName { get; set; }
        public bool IsPrivate { get; set; }
    }
}
