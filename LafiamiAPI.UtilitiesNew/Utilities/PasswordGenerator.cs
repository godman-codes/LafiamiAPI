using System.Text;

namespace LafiamiAPI.Utilities.Utilities
{
    public class PasswordGenerator
    {
        public static string GetRandomPassword()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomGenerator.GetRandomString(5, false));
            builder.Append(RandomGenerator.GetRandomNumber(10, 500));
            builder.Append(RandomGenerator.GetRandomString(3, true));
            builder.Append("#");

            return builder.ToString();
        }


    }
}
