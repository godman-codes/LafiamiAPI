using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace LafiamiAPI.Utilities.Utilities
{
    public class HTMLScrubber
    {
        public static string ScrubHtml(string value)
        {
            value = HttpUtility.HtmlDecode(value);

            value = RemoveTag(value, "<!--", "-->");
            value = RemoveTag(value, "<script", "</script>");
            value = RemoveTag(value, "<style", "</style>");

            string step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
            string step2 = Regex.Replace(step1, @"\s{2,}", " ");
            step2 = SingleSpacedTrim(step2);
            return step2;
        }

        private static string RemoveTag(string html, string startTag, string endTag)
        {
            bool bAgain;
            do
            {
                bAgain = false;
                int startTagPos = html.IndexOf(startTag, 0, StringComparison.CurrentCultureIgnoreCase);
                if (startTagPos < 0)
                {
                    continue;
                }

                int endTagPos = html.IndexOf(endTag, startTagPos + 1, StringComparison.CurrentCultureIgnoreCase);
                if (endTagPos <= startTagPos)
                {
                    continue;
                }

                html = html.Remove(startTagPos, endTagPos - startTagPos + endTag.Length);
                bAgain = true;
            } while (bAgain);
            return html;
        }

        private static string SingleSpacedTrim(string inString)
        {
            StringBuilder sb = new StringBuilder();
            bool inBlanks = false;
            foreach (char c in inString)
            {
                switch (c)
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case ' ':
                        if (!inBlanks)
                        {
                            inBlanks = true;
                            sb.Append(' ');
                        }
                        continue;
                    default:
                        inBlanks = false;
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString().Trim();
        }
    }
}
