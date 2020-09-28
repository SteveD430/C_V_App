using System.IO.Ports;

namespace C_V_App.SerialPortWrappers
{
    public enum SerialPortStates { Open = 0, Closed = 1 }

    [System.ComponentModel.DesignerCategory("Code")]
    public class WrappedSerialPort : SerialPort, ISerialPort
    {
        public WrappedSerialPort (string name) : base(name)
        {

        }

        public virtual new int BaudRate
        {
            get { return base.BaudRate; }
            set { base.BaudRate = value; }
        }

        public virtual new int DataBits
        {
            get { return base.DataBits; }
            set { base.DataBits = value; }
        }

        public virtual new int ReadTimeout
        {
            get { return base.ReadTimeout; }
            set { base.ReadTimeout = value; }
        }

        public virtual new int WriteTimeout
        {
            get { return base.WriteTimeout; }
            set { base.WriteTimeout = value; }
        }

        public virtual new int ReadBufferSize
        {
            get { return base.ReadBufferSize; }
            set { base.ReadBufferSize = value; }
        }

        public virtual new int WriteBufferSize
        {
            get { return base.WriteBufferSize; }
            set { base.WriteBufferSize = value; }
        }

        public virtual new bool DtrEnable
        {
            get { return base.DtrEnable; }
            set { base.DtrEnable = value; }
        }

        public virtual new bool RtsEnable
        {
            get { return base.RtsEnable; }
            set { base.RtsEnable = value; }
        }

        public virtual new bool IsOpen
        {
            get { return base.IsOpen; }
        }

        public virtual new string PortName
        {
            get { return base.PortName; }
            set { base.PortName = value; }
        }

        public virtual new string NewLine
        {
            get { return base.NewLine; }
            set { base.NewLine = value; }
        }

        public virtual new Parity Parity
        {
            get { return base.Parity; }
            set { base.Parity = value; }
        }

        public virtual new StopBits StopBits
        {
            get { return base.StopBits; }
            set { base.StopBits = value; }
        }

        public virtual new Handshake Handshake
        {
            get { return base.Handshake; }
            set { base.Handshake = value; }
        }

    }
}