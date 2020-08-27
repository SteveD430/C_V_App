using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Threading;


namespace C_V_App.SerialPortWrappers
{
    public class SerialPortManager : ISerialPortManager
    {
        private IDictionary<string, ISerialPort> _serialPorts;

        public SerialPortManager()
        {
            _serialPorts = new Dictionary<string, ISerialPort>();

            foreach (var portName in SerialPort.GetPortNames())
            {
                _serialPorts.Add(portName, new WrappedSerialPort(portName));
            }
        }

        public IEnumerable<string> GetNotAllocatedPortNames()
        {
            ISerialPort port;
            var portNames = new List<string>();
            foreach (var portName in SerialPort.GetPortNames())
            {
                if (_serialPorts.TryGetValue(portName, out port) && !port.IsOpen)
                {
                    portNames.Add(portName);
                }
            }
            return portNames;
        }

        public ISerialPort GetSerialPort(string name)
        {
            return new WrappedSerialPort(name);
        }
    }
}
