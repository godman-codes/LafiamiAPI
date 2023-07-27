namespace LafiamiAPI.Utilities.Extensions
{
    public static class DecimalExtension
    {
        public static string CurrencyNumberToWord(this decimal val, string notationInWord, string afterDotNotationInWord)
        {
            string valInString = val.ToString();
            if (valInString.Contains(Constants.Constants.Dot))
            {
                string[] splitted = valInString.Split(Constants.Constants.Dot);
                if (splitted.Length == Constants.Constants.One)
                {
                    return (long.Parse(splitted[Constants.Constants.Zero]).NumberToWord(long.Parse(splitted[Constants.Constants.Zero])));
                }

                if (splitted.Length == Constants.Constants.Two)
                {
                    if (long.Parse(splitted[Constants.Constants.One]) <= Constants.Constants.Zero)
                    {
                        return string.Join(Constants.Constants.Space, (long.Parse(splitted[Constants.Constants.Zero]).NumberToWord(long.Parse(splitted[Constants.Constants.Zero]))), notationInWord);
                    }

                    string dividerNotationInWord = string.Join(Constants.Constants.Space, string.Empty, notationInWord, Constants.Constants.And, string.Empty);
                    return string.Join(Constants.Constants.Space, (string.Join(string.Empty, long.Parse(splitted[Constants.Constants.Zero]).NumberToWord(long.Parse(splitted[Constants.Constants.Zero])), dividerNotationInWord, long.Parse(splitted[Constants.Constants.One]).NumberToWord(long.Parse(splitted[Constants.Constants.One])))), afterDotNotationInWord);
                }
            }

            return (long.Parse(valInString).NumberToWord(long.Parse(valInString)));
        }

        public static string NumberToWord(this decimal val)
        {
            string valInString = val.ToString();
            if (valInString.Contains(Constants.Constants.Dot))
            {
                string[] splitted = valInString.Split(Constants.Constants.Dot);
                if (splitted.Length == Constants.Constants.One)
                {
                    return (long.Parse(splitted[Constants.Constants.Zero]).NumberToWord(long.Parse(splitted[Constants.Constants.Zero])));
                }

                if (splitted.Length == Constants.Constants.Two)
                {
                    if (long.Parse(splitted[Constants.Constants.One]) <= Constants.Constants.Zero)
                    {
                        return (long.Parse(splitted[Constants.Constants.Zero]).NumberToWord(long.Parse(splitted[Constants.Constants.Zero])));
                    }

                    string dividerNotationInWord = string.Join(Constants.Constants.And, Constants.Constants.Space, Constants.Constants.Space);
                    return (string.Join(string.Empty, long.Parse(splitted[Constants.Constants.Zero]).NumberToWord(long.Parse(splitted[Constants.Constants.Zero])), dividerNotationInWord, long.Parse(splitted[Constants.Constants.One]).NumberToWord(long.Parse(splitted[Constants.Constants.One]))));
                }
            }

            return (long.Parse(valInString).NumberToWord(long.Parse(valInString)));
        }
    }
}
