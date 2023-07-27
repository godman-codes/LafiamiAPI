using System;
using System.Collections.Generic;

namespace LafiamiAPI.Utilities.Extensions
{
    public static class LongExtension
    {
        private class ValPair
        {
            public long Val { get; set; }
            public long PreVal { get; set; }
        }

        private static readonly Func<ValPair, string> remainder = t => t.Val > 0 ? " " + ActualNumberToWord(t.Val, t.PreVal) : "";

        public static string NumberToWord(this long val, long? preval = null)
        {
            return val.ActualNumberToWord(preval ?? val);
        }

        private static string ActualNumberToWord(this long val, long preval, double d = 20, long th = 20)
        {
            switch ((long)d)
            {
                case 20: return val >= d ? ActualNumberToWord(val, preval, 1e2) : en[val];
                case 100: return val >= d ? ActualNumberToWord(val, preval, 1e3, 100) : ((((val % 100) < preval)) ? ("and " + en[val / 10 * 10]) : (en[val / 10 * 10])) + remainder(new ValPair() { Val = (val % 10), PreVal = val });
                default: return val >= d ? ActualNumberToWord(val, preval, d * 1e3, (long)d) : ActualNumberToWord((val / th), val) + " " + en[th] + remainder(new ValPair() { Val = (val % th), PreVal = val });
            }
        }

        private static readonly Dictionary<long, string> en = new Dictionary<long, string> {
        {0 , "zero"},
        {1 , "One"},
        {2 , "Two"},
        {3 , "Three"},
        {4 , "Four"},
        {5 , "Five"},
        {6 , "Six"},
        {7 , "Seven"},
        {8 , "Eight"},
        {9 , "Nine"},
        {10, "Ten"},
        {11, "Eleven"},
        {12, "Twelve"},
        {13, "Thirteen"},
        {14, "Fourteen"},
        {15, "Fifteen"},
        {16, "Sixteen"},
        {17, "Seventeen"},
        {18, "Eighteen"},
        {19, "Nineteen"},
        {20, "Twenty"},
        {30, "Thirty"},
        {40, "Forty"},
        {50, "Fifty"},
        {60, "Sixty"},
        {70, "Seventy"},
        {80, "Eighty"},
        {90, "Ninety"},
        {100,"hundred"},
        {(long)1e3,  "thousand"},
        {(long)1e6,  "million"},
        {(long)1e9,  "billion"},
        {(long)1e12, "trillion"},
        {(long)1e15, "quadrillion"},
        {(long)1e18, "quintillion"}
    };

    }
}
