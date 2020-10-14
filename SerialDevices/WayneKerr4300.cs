using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using C_V_App.SerialPortWrappers;
using C_V_App.Exceptions;
using C_V_App.MeasurementDevices;

namespace C_V_App.SerialDevices
{
    public class WayneKerr4300 : BaseSerialDevice, ISerialController
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
        private const Handshake HAND_SHAKE = Handshake.RequestToSendXOnXOff;

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
        public override string Title => "WAYNE KERR 4300";

        public override string Description => "WAYNE KERR 4300";

        public override ISerialPort SerialPort { get; protected set; }

        public override void InitializeDevice(ISerialPortManager serialPortManager)
        {
            _serialPortManager = serialPortManager ?? throw new ArgumentNullException(nameof(serialPortManager));
            if (DiscoverDevice(_serialPortManager))
            {
                ConfigurePort(SerialPort);
            }
            else
            {
                throw new DeviceNotFoundException("No Wayne Kerr 4300 Device available");
            }
        }

        public override void ReleaseDevice()
        {
            base.ReleaseDevice();
        }

        #endregion ISerialDevice

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

                    if (response.Contains("WAYNE"))
                    {
                        SerialPort = serialPort;
                        return true;
                    }
                }
                catch
                {
                    // Ignore port error. 
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
    }
}
