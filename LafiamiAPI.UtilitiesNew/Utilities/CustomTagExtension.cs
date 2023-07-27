using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Enums;
using LafiamiAPI.Utilities.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LafiamiAPI.Utilities.Utilities
{
    public static class CustomTagExtension
    {
        public static List<CustomTag> DefaultSubjectEmailTypeTags()
        {
            string webRootPath = AppDomain.CurrentDomain.GetData(Constants.Constants.WebRootPath) as string;
            List<CustomTag> tags = Enum.GetValues(typeof(EmailTypeEnums)).Cast<EmailTypeEnums>().Select(r => new CustomTag
            {
                TagName = r.ToString(),
                ContentPath = Path.Combine(Path.Combine(webRootPath, CustomPath.EmailSubjects), (r.ToString() + PathExtension.Html)),
            }).ToList();


            return tags;
        }

        public static List<CustomTag> DefaultTemplateEmailTypeTags()
        {
            string webRootPath = AppDomain.CurrentDomain.GetData(Constants.Constants.WebRootPath) as string;
            List<CustomTag> tags = Enum.GetValues(typeof(EmailTypeEnums)).Cast<EmailTypeEnums>().Select(r => new CustomTag
            {
                TagName = r.ToString(),
                ContentPath = Path.Combine(Path.Combine(webRootPath, CustomPath.EmailTemplates), (r.ToString() + PathExtension.Html)),
            }).ToList();

            return tags;
        }

        public static List<string> GetDefinedTags(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return new List<string>();
            }

            string nextpagecontent = HTMLScrubber.ScrubHtml(content);
            List<string> result = new List<string>();

            bool status;
            do
            {
                Match wt = Regex.Match(nextpagecontent, DelimiterResolver.StartPattern, RegexOptions.IgnoreCase);
                if (wt.Success)
                {
                    status = true;
                    Match emt = Regex.Match(wt.Value, DelimiterResolver.EndPattern);
                    if (emt.Success)
                    {
                        string pointername = wt.Value.Substring(DelimiterResolver.PatternCount, (emt.Index - DelimiterResolver.PatternCount));
                        string pointerwithdelimiter = wt.Value.Substring(0, (emt.Index + emt.Length));
                        if (pointername.Contains(DelimiterResolver.SplitCharacter))
                        {
                            string[] split = pointername.Split(DelimiterResolver.SplitCharacter);
                            result.Add(split[0]);
                        }
                        else
                        {
                            result.Add(pointername);
                        }
                        nextpagecontent = wt.Value.Replace(pointerwithdelimiter, "");
                    }
                }
                else
                {
                    status = false;
                }

            } while (status);

            return result;
        }
    }
}
