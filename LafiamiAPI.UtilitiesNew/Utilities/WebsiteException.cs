using System;

namespace LafiamiAPI.Utilities.Utilities
{
    public class WebsiteException : Exception
    {
        public WebsiteException()
        {

        }

        public WebsiteException(string message) : base(message)
        {

        }

        public WebsiteException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
