using System.Collections.Generic;
using C_V_App.SerialDevices;

namespace C_V_App.Models
{
    public interface IKeithley2400Model : ISerialDevice
    {
        void Initialize(KEITHLEY_CONFIG keithley_config);

        IList<string> ReportingFields { get; }

        double StartVoltage { get;  set; }

        double FinalVoltage { get;  set; }

        double IncrementVoltage { get;  set; }

        double CurrentLimit { get;  set; }

    }
}
