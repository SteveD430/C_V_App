using System;
using System.Collections.Generic;
using System.Threading;
using C_V_App.SerialPortWrappers;
using C_V_App.SerialDevices;

namespace C_V_App.Models
{
    public class WayneKerr4300Model : IWayneKerr4300Model
    {
        WayneKerr4300 _wayneKerr4300SerialDevice;

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

        public WayneKerr4300Model()
        {
            DeviceIdentifier = "Wayne";
            _reportingFields = new List<string>();
            _wayneKerr4300SerialDevice = new WayneKerr4300();
        }

        public double Amplitude { get; set; }

        public decimal Limit { private get; set; }

        public string DeviceIdentifier { get; private set; }

        public string Title => _wayneKerr4300SerialDevice.Title;

        public string Description => _wayneKerr4300SerialDevice.Description;

        public ISerialPort SerialPort => _wayneKerr4300SerialDevice.SerialPort;

        public bool DeviceAvailable => _wayneKerr4300SerialDevice.DeviceAvailable;

        public IList<string> ReportingFields => _wayneKerr4300SerialDevice.ReportingFields;

        public void Initialize()
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

        public void SerialSafeWrite(string data)
        {
            if(!_wayneKerr4300SerialDevice.SerialPort.IsOpen)
            {
                _wayneKerr4300SerialDevice.SerialPort.Open();
            }
            _wayneKerr4300SerialDevice.SerialSafeWrite(data);
        }

        public string SerialSafeRead(string request)
        {
            return _wayneKerr4300SerialDevice.SerialSafeRead(request);
        }


        public void ClearBuffers()
        {
            _wayneKerr4300SerialDevice.ClearBuffers();
        }

        public void InitializeDevice(ISerialPortManager serialPortManager)
        {
            _wayneKerr4300SerialDevice.InitializeDevice(serialPortManager);
        }

        public void ReleaseDevice()
        {
            _wayneKerr4300SerialDevice.ReleaseDevice();
        }

        public void Dispose()
        {
            _wayneKerr4300SerialDevice.Dispose();
        }
    }
}
