using System;

namespace C_V_App.Exceptions
{
    [Serializable]
    public class UnrecognisedMeasurementDimensionException : Exception
    {
        public UnrecognisedMeasurementDimensionException()
        {
        }

        public UnrecognisedMeasurementDimensionException(string message)
            : base(message)
        {
        }

        public UnrecognisedMeasurementDimensionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
