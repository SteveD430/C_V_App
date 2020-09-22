using System;

namespace C_V_App.Exceptions
{
    [Serializable]
    public class EmulationFileNotFoundException : Exception
    {
        public EmulationFileNotFoundException()
        {
        }

        public EmulationFileNotFoundException(string message)
            : base(message)
        {
        }

        public EmulationFileNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
