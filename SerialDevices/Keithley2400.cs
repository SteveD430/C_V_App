using System;
using System.IO.Ports;
using System.Threading;
using C_V_App.Exceptions;
using C_V_App.SerialPortWrappers;

namespace C_V_App.SerialDevices
{
    /// <summary>
    /// KT2400 is a KEITHLEY 2400 Controller.
    /// Two possible configurations:
    ///     Set Voltage, measure current.
    ///     Set Amps, measure voltage.
    /// </summary>
    public class Keithley2400 : BaseSerialDevice, ISerialController
    {

        private const int DATA_BITS = 8;
        private const int BAUD_RATE = 9600;
        private const int MESSAGE_DELAY = 250;
        private const int READ_TIMEOUT = 7500;
        private const int WRITE_TIMEOUT = 7500;
        private const int READ_BUFFER_SIZE = 1500;
        private const int WRITE_BUFFER_SIZE = 1000;

        private const Parity PARITY = Parity.None;
        private const StopBits STOP_BITS = StopBits.One;
        private const Handshake HAND_SHAKE = Handshake.XOnXOff;

 
        private ISerialPortManager _serialPortManager;

        #region ISerialController
        public void RequestData()
        {
            SerialPort.WriteLine(":READ?");
        }

        public string ReadData()
        {
            return SerialPort.ReadLine();
        }
        #endregion ISerialController

        #region ISerialDevice
        public override string Title => "KEITHLEY 2400";

        public override string Description => "KEITHLEY 2400";

        public override ISerialPort SerialPort { get; protected set; }

         public override void InitializeDevice(ISerialPortManager serialPortManager)
        {
            if (serialPortManager == null)
            {
                throw new ArgumentNullException(nameof(serialPortManager));
            }
            _serialPortManager = serialPortManager;

            if (DiscoverDevice(_serialPortManager))
            {
                ConfigurePort(SerialPort);
            }
            else
            {
                throw new DeviceNotFoundException("No Keithley 2400 Device available");
            }
        }

        public override void ReleaseDevice()
        {
            if (SerialPort != null)
            {
                if (!SerialPort.IsOpen)
                {
                    SerialPort.Open();
                }
                SerialPort.WriteLine("SOUR:VOLT:LEV 0");    // set the voltage level to zero
                SerialPort.WriteLine(":OUTP OFF");          // and turn the Keithley supply off
            }
            base.ReleaseDevice();
        }

        private bool DiscoverDevice(ISerialPortManager serialPortManager)
        {
            ISerialPort serialPort;
            string response;// just used for testPort receive

            foreach (var portName in serialPortManager.GetNotAllocatedPortNames())
            {
                serialPort = serialPortManager.GetSerialPort(portName);
                ConfigurePortForDiscovery(serialPort);
                Thread.Sleep(MESSAGE_DELAY);

                try
                {
                    serialPort.WriteLine("*IDN?");
                    response = serialPort.ReadLine().ToUpper();
                    serialPort.Close();

                    if (response.Contains("KEITHLEY") && response.Contains("2400"))
                    {
                        SerialPort = serialPort;
                        return true;
                    }

                }
                catch 
                {
                    // Ignore Port error. throw new PortCommunicationException(ex.Message, ex);
                }
            }
            return false;
        }

        private void ConfigurePortForDiscovery(ISerialPort serialPort)
        {

            serialPort.ReadBufferSize = READ_BUFFER_SIZE;
            serialPort.WriteBufferSize = WRITE_BUFFER_SIZE;

            serialPort.BaudRate = BAUD_RATE;
            serialPort.NewLine = NEWLINE;
            serialPort.StopBits = STOP_BITS;
            serialPort.DataBits = DATA_BITS;
            serialPort.Parity = PARITY;

            //Not sure why the following three are differnet during discovery.
            serialPort.Handshake = Handshake.RequestToSendXOnXOff; //Because it works for either K24 or WK43
            serialPort.ReadTimeout = 1000;
            serialPort.WriteTimeout = 1000;

            serialPort.Open();
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }

        private void ConfigurePort(ISerialPort serialPort)
        {
            serialPort.BaudRate = BAUD_RATE;
            serialPort.NewLine = NEWLINE;
            serialPort.StopBits = STOP_BITS;
            serialPort.DataBits = DATA_BITS;
            serialPort.Parity = PARITY;

            serialPort.Handshake = HAND_SHAKE;
            serialPort.ReadTimeout = READ_TIMEOUT;
            serialPort.WriteTimeout = WRITE_TIMEOUT;

            serialPort.Open();
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
        }
        #endregion ISerialDevice
     }
}
