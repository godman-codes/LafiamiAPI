using LafiamiAPI.Utilities.Enums;
using System;
using System.Linq;

namespace LafiamiAPI.Utilities.Utilities
{
    public static class DefaultTemplates
    {
        public static string GetEmailTemplate(EmailTypeEnums emailType)
        {
            string rawContent = DelimiterResolver.AppendDelimiters(emailType.ToString());
            System.Collections.Generic.List<Models.CustomTag> defaultTags = CustomTagExtension.DefaultTemplateEmailTypeTags();
            Models.CustomTag customTag = defaultTags.Where(r => r.TagName.Equals(emailType.ToString(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            return new CustomTagsResolver()
            {
                RawContent = rawContent,
                StartPattern = customTag.TagNameWithStartPattern,
                StartDelimiterLength = customTag.StartDelimiterLength,
                TemplateContentpath = customTag.ContentPath,
                //HasCallToAction = customTag.HasCallToAction,
                //DefaultCallToActionName = customTag.DefaultCallToActionName,
            }.FinalContent;
        }
        public static string GetEmailSubject(EmailTypeEnums emailType)
        {
            string rawContent = DelimiterResolver.AppendDelimiters(emailType.ToString());
            System.Collections.Generic.List<Models.CustomTag> defaultTags = CustomTagExtension.DefaultSubjectEmailTypeTags();
            Models.CustomTag customTag = defaultTags.Where(r => r.TagName.Equals(emailType.ToString(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            return new CustomTagsResolver()
            {
                RawContent = rawContent,
                StartPattern = customTag.TagNameWithStartPattern,
                StartDelimiterLength = customTag.StartDelimiterLength,
                TemplateContentpath = customTag.ContentPath,
                //HasCallToAction = customTag.HasCallToAction,
                //DefaultCallToActionName = customTag.DefaultCallToActionName,
            }.FinalContent;
        }
    }
}
