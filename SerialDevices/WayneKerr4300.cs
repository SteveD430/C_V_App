using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using C_V_App.SerialPortWrappers;
using C_V_App.Exceptions;

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

        private IList<string> _reportingFields;

        private string[] WK_FUNCTIONS = { "Capacitance",
        "Inductance",
        "Reactance",
        "Susceptance",
        "Impedance",
        "Admittance",
        "Quality factor",
        "Dissipation factor",
        "Resistance",
        "Conductance",
        "DC Resistance"
        };

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

        public IList<string> ReportingFields { get; private set; }

        public override void InitializeDevice(ISerialPortManager serialPortManager)
        {
            _serialPortManager = serialPortManager ?? throw new ArgumentNullException(nameof(serialPortManager));
            _reportingFields = new List<string>();
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
                catch (Exception ex)
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

        private void Initialize(ISerialPortManager serialPortManager)
        {
            // Initialization of the WK4300: see Project for C_V_105 for original source.
            SerialSafeWrite("MEAS:EQU-CCT PAR");
            SerialSafeWrite(":MEAS:SPEED SLOW ");

            SerialSafeWrite(":MEAS:FUNC1 C");
            Thread.Sleep(250);

            SerialSafeWrite(":MEAS:FUNC2 D");
            SerialSafeWrite(":MEAS:FREQ 0.5e6");
            Thread.Sleep(1000);

            // Get Reporting Field names;
            _reportingFields.Add(GetReportField(":MEAS:FUNC1?"));
            _reportingFields.Add(GetReportField(":MEAS:FUNC2?"));
        }

        private string GetReportField(string fieldRequest)
        {
            int functionId;
            string functionEntry = SerialSafeRead(fieldRequest);
            if (Int32.TryParse(functionEntry, out functionId) && functionId >= 0 && functionId < WK_FUNCTIONS.Length)
            {
                return WK_FUNCTIONS[functionId];
            }
            else
            {
                return "Unknown Function";
            }
        }
    }
}
