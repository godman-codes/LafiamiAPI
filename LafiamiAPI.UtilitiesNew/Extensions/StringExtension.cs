using LafiamiAPI.Utilities.Constants;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LafiamiAPI.Utilities.Extensions
{
    public static class StringExtension
    {
        public static string EqualJoin(this string name, object value)
        {
            return name + Constants.Constants.EqualTo + value;
        }

        public static void GetIndexes(this string content, string startPattern, string endPattern, ref int startindex, ref int endindex)
        {
            Match st = Regex.Match(content, startPattern, RegexOptions.IgnoreCase);
            Match et = Regex.Match(content, endPattern, RegexOptions.IgnoreCase);
            if (st.Success && et.Success)
            {
                var startingendindex = et.Index;
                var lastendindex = et.Index;
                bool nextcheck;

                do
                {
                    if (st.Index < startingendindex)
                    {
                        if (startindex < 0)
                        {
                            startindex = st.Index;
                        }
                        lastendindex = et.Index;

                        st = st.NextMatch();
                        et = et.NextMatch();

                        nextcheck = (st.Success && et.Success);
                        if (!nextcheck)
                        {
                            endindex = lastendindex;
                        }
                    }
                    else
                    {
                        endindex = et.Index;
                        nextcheck = false;
                    }
                } while (nextcheck);
            }
        }

    }
}
