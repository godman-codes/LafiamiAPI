using System;
using System.Text;

namespace LafiamiAPI.Utilities.Utilities
{
    public class RandomGenerator
    {
        public static int GetRandomNumber(int start, int end)
        {
            Random rand = new Random();
            return rand.Next(start, end);
        }

        public static string GetRandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random rand = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(26 * rand.NextDouble() + 65));
                builder.Append(ch);
            }

            return ((lowerCase) ? (builder.ToString().ToLower()) : (builder.ToString()));

        }
    }
}
