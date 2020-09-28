using System;
using System.IO;
using System.IO.Ports;

namespace C_V_App.SerialPortWrappers
{
    public class WrappedEmulatorSerialPort : WrappedSerialPort, ISerialPort
    {
        public WrappedEmulatorSerialPort (string name) : base(name)
        {

        }

        protected StreamWriter Debug { get; set; }
        // ISerialPort
        // ===========
        public override int BaudRate
        {
            get { return base.BaudRate; }
            set
            {
                base.BaudRate = value;
                Debug.WriteLine($"BaudRate {value}");
            }
        }

        public override int DataBits
        {
            get { return base.DataBits; }
            set
            {
                base.BaudRate = value;
                Debug.WriteLine($"DataBits {value}");
            }
        }

        public override int ReadTimeout
        {
            get { return base.ReadTimeout; }
            set
            {
                base.BaudRate = value;
                Debug.WriteLine($"ReadTimeout {value}");
            }
        }

        public override int WriteTimeout
        {
            get { return base.WriteTimeout; }
            set
            {
                base.BaudRate = value;
                Debug.WriteLine($"WriteTimeout {value}");
            }
        }

        public override int ReadBufferSize
        {
            get { return base.ReadBufferSize; }
            set
            {
                base.BaudRate = value;
                Debug.WriteLine($"ReadBufferSize {value}");
            }
        }

        public override int WriteBufferSize
        {
            get { return base.WriteBufferSize; }
            set
            {
                base.BaudRate = value;
                Debug.WriteLine($"WriteBufferSize {value}");
            }
        }

        public override bool DtrEnable
        {
            get { return base.DtrEnable; }
            set
            {
                base.DtrEnable = value;
                Debug.WriteLine($"DtrEnable {value}");
            }
        }

        public override bool RtsEnable
        {
            get { return base.RtsEnable; }
            set
            {
                base.RtsEnable = value;
                Debug.WriteLine($"RtsEnable {value}");
            }
        }

        public override string PortName
        {
            get { return base.PortName; }
            set
            {
                base.PortName = value;
                Debug.WriteLine($"RtsEnable {value}");
            }
        }

        public override string NewLine
        {
            get { return base.NewLine; }
            set
            {
                base.NewLine = value;
                Debug.WriteLine($"NewLine {value}");
            }
        }

        public override Parity Parity
        {
            get { return base.Parity; }
            set
            {
                base.Parity = value;
                Debug.WriteLine($"Parity {value}");
            }
        }

        public override StopBits StopBits
        {
            get { return base.StopBits; }
            set
            {
                base.StopBits = value;
                Debug.WriteLine($"StopBits {value}");
            }
        }

        public override Handshake Handshake
        {
            get { return base.Handshake; }
            set
            {
                base.Handshake = value;
                Debug.WriteLine($"Handshake {value}");
            }
        }

    }
}
