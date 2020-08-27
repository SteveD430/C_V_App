using System;
using System.Collections.Generic;
using C_V_App.SerialPortWrappers;
using C_V_App.SerialDevices;

namespace C_V_App.Models
{
    public class WayneKerr4300Model : IWayneKerr4300Model
    {
        WayneKerr4300 _wayneKerr4300SerialDevice;

        public WayneKerr4300Model()
        {
            DeviceIdentifier = "Wayne";
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

        public void SerialSafeWrite(string data)
        {
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
