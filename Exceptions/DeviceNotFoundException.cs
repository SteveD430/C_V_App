using System;

namespace C_V_App.Exceptions
{
    public class DeviceNotFoundException : Exception
    {
        public DeviceNotFoundException()
        {
        }

        public DeviceNotFoundException(string message)
            : base(message)
        {
        }

        public DeviceNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
