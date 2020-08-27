using System;
using System.IO;

namespace C_V_App.SerialPortWrappers
{
    public interface ISerialPortEmulator : ISerialPort
    {
        string EmulatorFileName { get; set; }
        StreamReader EmulationDataStream { get; set; }
    }
}
