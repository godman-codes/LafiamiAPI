using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Utilities.Utilities
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException()
        {

        }

        public ForbiddenException(string message) : base(message)
        {

        }

        public ForbiddenException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
