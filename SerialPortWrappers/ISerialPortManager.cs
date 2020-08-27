using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace C_V_App.SerialPortWrappers
{
    public interface ISerialPortManager
    {

        IEnumerable<string> GetNotAllocatedPortNames();

        ISerialPort GetSerialPort(string name);
    }
}
