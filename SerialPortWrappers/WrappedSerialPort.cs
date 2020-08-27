using System.IO.Ports;

namespace C_V_App.SerialPortWrappers
{
    public enum SerialPortStates { Open = 0, Initialised = 1, Closed = 2 }

    [System.ComponentModel.DesignerCategory("Code")]
    public class WrappedSerialPort : SerialPort, ISerialPort
    {
        public WrappedSerialPort (string name) : base(name)
        {

        }

        public new string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }
    }
}