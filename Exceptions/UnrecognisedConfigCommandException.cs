using System;

namespace C_V_App.Exceptions
{
    public class UnrecognisedConfigCommandException : Exception
    {
        public UnrecognisedConfigCommandException()
        {
        }

        public UnrecognisedConfigCommandException(string message)
            : base(message)
        {
        }

        public UnrecognisedConfigCommandException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
