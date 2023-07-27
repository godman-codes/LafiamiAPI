namespace LafiamiAPI.Utilities.Utilities
{
    public class DelimiterResolver
    {
        private const string startDel = @"[[";
        private const string endDel = "]]";
        private const string startval = @"\[\[";
        private const string endval = @".*";
        private const char splitCharacter = '=';

        public static char SplitCharacter
        {
            get
            {
                return splitCharacter;
            }
        }

        public static string StartPattern
        {
            get
            {
                return startval + endval;
            }
        }

        public static string EndPattern
        {
            get
            {
                return @"\]\]";
            }
        }
        public static int PatternCount
        {
            get
            {
                return 2;
            }
        }

        public static string StartPatternWithName(string value)
        {
            return (startval + value + endval);
        }
        public static string AppendDelimiters(string value)
        {
            return (startDel + value + endDel);
        }

    }
}
