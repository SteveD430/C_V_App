using System.Collections.Generic;
using C_V_App.SerialDevices;

namespace C_V_App.Models
{
    public interface IWayneKerr4300Model : ISerialDevice
    {
        IList<string> ReportingFields { get; }

        double Amplitude { get; set; }

        void Initialize();

    }
}
