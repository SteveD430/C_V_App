using System.Collections.Generic;
using System.Linq;

namespace C_V_App.SerialPortWrappers
{
    public class SerialPortEmulatorManager : ISerialPortManager
    {
        private const string COM1 = "COM1";
        private const string COM2 = "COM2";
        private ICVEnvironment _CVEnvironment;
        private readonly Dictionary<string, ISerialPortEmulator> _portEmulators;
        private readonly string[] _ports;

        public SerialPortEmulatorManager()
        {
            _ports = new string[] { COM1, COM2 };
            _portEmulators = new Dictionary<string, ISerialPortEmulator>()
            {
                {_ports[0], new Keithley2400Emulator(_ports[0]) },
                {_ports[1], new WayneKerr4300Emulator(_ports[1]) }
            };
        }

        public IEnumerable<string> GetPortNames()
        {
            return _ports;
        }

        public IEnumerable<string> GetNotAllocatedPortNames()
        {
            ISerialPortEmulator port;
            List<string> notAllocatedPorts = new List<string>();
            foreach (var portName in _ports)
            {
                if (_portEmulators.TryGetValue(portName, out port) && !port.IsOpen)
                {
                    notAllocatedPorts.Add(portName);
                }
            }
            return notAllocatedPorts;
        }

        public ISerialPort GetSerialPort(string name)
        {
            ISerialPortEmulator port;
            if( _portEmulators.TryGetValue(name, out port))
            {
                return port;
            }
            return null;
        }

        // Emulator Specific functions
        public ICVEnvironment EmulatorEnvironment
        {
            get { return _CVEnvironment; }
            set
            {
                _CVEnvironment = value;
                _portEmulators[COM1].EmulatorFileName = _CVEnvironment.KeithleyEmulationFilename;
                _portEmulators[COM2].EmulatorFileName = _CVEnvironment.WayneKerrEmulationFilename;
            }
        }
    }
}
