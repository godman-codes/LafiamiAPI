using LafiamiAPI.Utilities.Utilities;

namespace LafiamiAPI.Utilities.Models
{
    public class CustomTag
    {
        public string TagName { get; set; }
        public string ContentPath { get; set; }
        //public bool HasCallToAction { get; set; }
        //public string DefaultCallToActionName { get; set; }

        public string TagNameWithStartPattern
        {
            get
            {
                if (string.IsNullOrEmpty(TagName)) { return string.Empty; }
                return DelimiterResolver.StartPatternWithName(TagName);
            }
        }
        public int StartDelimiterLength
        {
            get
            {
                if (string.IsNullOrEmpty(TagName)) { return DelimiterResolver.PatternCount; }
                return TagName.Length + DelimiterResolver.PatternCount;
            }
        }
    }
}
