using System.Collections.Generic;


namespace C_V_App.MeasurementDevices
{
    public interface IMeasurementDevice
    {
        ICollection<string> RecognisedDimensions();

        void Measure(string dimension);

        IEnumerable<string> Headings();

        IEnumerable<double> Measurements();
    }
}
