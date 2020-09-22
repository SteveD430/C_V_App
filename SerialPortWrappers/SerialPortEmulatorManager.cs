using System.Collections.Generic;
using System.Linq;

namespace C_V_App.SerialPortWrappers
{
    public class SerialPortEmulatorManager : ISerialPortManager
    {
        private const string COM1 = "COM1";
        private const string COM2 = "COM2";
        private ICVEnvironment _CVEnvironment;
        private Dictionary<string, ISerialPortEmulator> _portEmulators;
        private readonly string[] _ports;

        public SerialPortEmulatorManager()
        {
            _ports = new string[] { COM1, COM2 };

        }

        public IEnumerable<string> GetPortNames()
        {
            return _ports;
        }

        private Dictionary<string, ISerialPortEmulator> PortEmulators
        {
            get
            {
                if (_portEmulators == null)
                {
                    _portEmulators = new Dictionary<string, ISerialPortEmulator>()
                    {
                        {_ports[0], new Keithley2400Emulator(_ports[0]) },
                        {_ports[1], new WayneKerr4300Emulator(_ports[1]) }
                    };
                }
                return _portEmulators;
            }
        }



        public IEnumerable<string> GetNotAllocatedPortNames()
        {
            ISerialPortEmulator port;
            List<string> notAllocatedPorts = new List<string>();
            foreach (var portName in _ports)
            {
                if (PortEmulators.TryGetValue(portName, out port) && !port.IsOpen)
                {
                    notAllocatedPorts.Add(portName);
                }
            }
            return notAllocatedPorts;
        }

        public ISerialPort GetSerialPort(string name)
        {
            ISerialPortEmulator port;
            if(PortEmulators.TryGetValue(name, out port))
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
                PortEmulators[COM1].EmulatorFileName = _CVEnvironment.KeithleyEmulationFilename;
                PortEmulators[COM2].EmulatorFileName = _CVEnvironment.WayneKerrEmulationFilename;
            }
        }
    }
}
