using System;

namespace C_V_App.Exceptions
{
    [Serializable]
    public class RetreivingDataException : Exception
    {
        public RetreivingDataException()
        {
        }

        public RetreivingDataException(string message)
            : base(message)
        {
        }

        public RetreivingDataException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
