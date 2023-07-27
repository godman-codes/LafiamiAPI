using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace LafiamiAPI.Utilities.Extensions
{
    public static class Extensions
    {
        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        public static string DisplayName(this Enum val)
        {
            DisplayAttribute enumObj = val.GetAttribute<DisplayAttribute>();
            return ((enumObj == null) ? val.ToString() : enumObj.Name);
        }

        public static string ShortName(this Enum val)
        {
            DisplayAttribute enumObj = val.GetAttribute<DisplayAttribute>();
            return ((enumObj == null) ? val.ToString() : enumObj.ShortName);
        }

        public static string Prompt(this Enum val)
        {
            DisplayAttribute enumObj = val.GetAttribute<DisplayAttribute>();
            return ((enumObj == null) ? val.ToString() : enumObj.Prompt);
        }

        public static bool AutoGenerateFilter(this Enum val)
        {
            DisplayAttribute enumObj = val.GetAttribute<DisplayAttribute>();
            return ((enumObj == null) ? false : enumObj.AutoGenerateFilter);
        }

        public static string GroupName(this Enum val)
        {
            DisplayAttribute enumObj = val.GetAttribute<DisplayAttribute>();
            return ((enumObj == null) ? val.ToString() : enumObj.GroupName);
        }

        public static string DisplayDescription(this Enum val)
        {
            DisplayAttribute enumObj = val.GetAttribute<DisplayAttribute>();
            return ((enumObj == null) ? val.ToString() : enumObj.Description);
        }

        public static int GetOrder(this Enum val)
        {
            DisplayAttribute enumObj = val.GetAttribute<DisplayAttribute>();
            return ((enumObj == null) ? 0 : enumObj.Order);
        }
    }
}
