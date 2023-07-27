using System.Text.RegularExpressions;

namespace LafiamiAPI.Utilities.Utilities
{
    public class CustomTagsResolver
    {
        public string RawContent { get; set; }
        public string StartPattern { get; set; }
        public int StartDelimiterLength { get; set; }
        public string TemplateContent { get; set; }
        public string TemplateContentpath { get; set; }

        // public bool HasCallToAction { get; set; }

        private string PointerName { get; set; }
        private string PointerWithDelimiter { get; set; }

        //public string CallToActionName { get; set; }
        //public string DefaultCallToActionName { get; set; }


        public string FinalContent
        {
            get
            {

                if (string.IsNullOrEmpty(RawContent))
                {
                    return string.Empty;
                }
                string finalContent = RawContent;

                Match myt = Regex.Match(finalContent, StartPattern, RegexOptions.IgnoreCase);
                if (myt.Success)
                {
                    Match emt = Regex.Match(myt.Value, DelimiterResolver.EndPattern, RegexOptions.IgnoreCase);
                    if (emt.Success)
                    {
                        PointerName = HTMLScrubber.ScrubHtml(myt.Value.Substring(StartDelimiterLength, (emt.Index - StartDelimiterLength)));
                        PointerWithDelimiter = myt.Value.Substring(0, (emt.Index + emt.Length));

                        if (!string.IsNullOrEmpty(TemplateContentpath))
                        {
                            TemplateContent = System.IO.File.ReadAllText(TemplateContentpath);
                        }

                        //if (HasCallToAction)
                        //{
                        //    CallToActionName = PointerName;
                        //    var callToActionDel = DelimiterResolver.AppendDelimiters(CustomTagName.CallToAction);
                        //    TemplateContent = ((string.IsNullOrEmpty(CallToActionName)) ? (TemplateContent.Replace(callToActionDel, DefaultCallToActionName)) : (TemplateContent.Replace(callToActionDel, CallToActionName)));
                        //}

                        finalContent = finalContent.Replace(PointerWithDelimiter, TemplateContent);
                    }
                }

                return finalContent;
            }
        }


    }
}
