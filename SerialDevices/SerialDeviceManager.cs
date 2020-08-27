using System;
using System.Collections.Generic;
using C_V_App.SerialPortWrappers;

namespace C_V_App.SerialDevices
{
    public enum SerialDeviceType { Controller, TempartureController, RelayTester };

    public class SerialDeviceManager
    {
        private static SerialDeviceManager _serialDeviceManager = new SerialDeviceManager();

        private Dictionary<SerialDeviceType, IList<ISerialDevice>> _devices;
        private SerialDeviceManager()
        {
            SerialPortManager serialPortManager = new SerialPortManager();
            Keithley2400I = new Keithley2400(serialPortManager, Keithley2400.KEITHLEY_CONFIG.I);
            Keithley2400V = new Keithley2400(serialPortManager, Keithley2400.KEITHLEY_CONFIG.V);
            WayneKerr4300 = new WayneKerr4300();

            _devices = new Dictionary<SerialDeviceType, IList<ISerialDevice>>();
            _devices.Add(SerialDeviceType.Controller, new List<ISerialDevice>());
            _devices.Add(SerialDeviceType.RelayTester, new List<ISerialDevice>());
            _devices.Add(SerialDeviceType.TempartureController, new List<ISerialDevice>());

            _devices[WayneKerr4300.SerialDeviceType].Add(WayneKerr4300);
            _devices[Keithley2400I.SerialDeviceType].Add(Keithley2400I);
            _devices[Keithley2400V.SerialDeviceType].Add(Keithley2400V);
        }

        public Keithley2400 Keithley2400I { get; private set; }

        public Keithley2400 Keithley2400V { get; private set; }

        public WayneKerr4300 WayneKerr4300 { get; private set; }


        public Dictionary<SerialDeviceType, IList<ISerialDevice>> DeviceList
        {
            get { return _devices; }
        }

        //public Temp Temp { get; private set; }
        //public Relay Relay { get; private set; }

        #region Singleton I/F
        public static Keithley2400 Keithley2400IModel
        {
            get { return _serialDeviceManager.Keithley2400I; }
        }

        public static Keithley2400 Keithley2400VModel
        {
            get { return _serialDeviceManager.Keithley2400V; }
        }

        public static WayneKerr4300 WayneKerr4300Model
        {
            get { return _serialDeviceManager.WayneKerr4300; }
        }

        public static Dictionary<SerialDeviceType, IList<ISerialDevice>> Devices
        {
            get { return _serialDeviceManager.DeviceList; }
        }
        #endregion Singleton I/F
    }
}
