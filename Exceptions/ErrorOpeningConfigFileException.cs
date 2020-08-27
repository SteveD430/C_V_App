using System;

namespace C_V_App.Exceptions
{
    public class ErrorOpeningConfigFileException : Exception
    {
        public ErrorOpeningConfigFileException()
        {
        }

        public ErrorOpeningConfigFileException(string message)
            : base(message)
        {
        }

        public ErrorOpeningConfigFileException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
