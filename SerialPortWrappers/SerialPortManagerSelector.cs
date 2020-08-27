using System;

namespace C_V_App.SerialPortWrappers
{
    public class SerialPortManagerSelector : ISerialPortManagerSelector
    {
        private ISerialPortManager _realSerialPortManager;
        private ISerialPortManager _serialPortEmulatorManager;

        public SerialPortManagerSelector(ISerialPortManager realSerialPortManager, ISerialPortManager serialPortEmulatorManager)
        {
            _realSerialPortManager = realSerialPortManager;
            _serialPortEmulatorManager = serialPortEmulatorManager;
        }


        public ISerialPortManager GetSerialManager(ICVEnvironment cvEnv)
        {
            if (cvEnv.Emulate)
            {
                ((SerialPortEmulatorManager)_serialPortEmulatorManager).EmulatorEnvironment = cvEnv;
                return _serialPortEmulatorManager;
            }
            return _realSerialPortManager;
        }
    }
}
