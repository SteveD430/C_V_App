using System;


namespace C_V_App.Exceptions
{
    [Serializable]
    public class EmulationFileReadException : Exception
    {
        public EmulationFileReadException()
        {
        }

        public EmulationFileReadException(string message)
            : base(message)
        {
        }

        public EmulationFileReadException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
