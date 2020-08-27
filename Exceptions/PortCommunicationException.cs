using System;

namespace C_V_App.Exceptions
{
    public class PortCommunicationException : Exception
    {
        public PortCommunicationException()
        {
        }

        public PortCommunicationException(string message)
            : base(message)
        {
        }

        public PortCommunicationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
