using System;
using C_V_App.SerialDevices;
using C_V_App.SerialPortWrappers;

namespace C_V_App.Models
{
    public enum KEITHLEY_CONFIG { V = 0, I = 1 };

    public class Keithley2400Model : IKeithley2400Model
    {
        private delegate void Keigthley2400Configuration();

        Keigthley2400Configuration[] _configurations;

        private ISerialDevice _keithley2400SerialDevice;

        public Keithley2400Model()
        {
            DeviceIdentifier = "Keithley";
            _keithley2400SerialDevice = new Keithley2400();
            _configurations = new Keigthley2400Configuration[]
            {
                    VSource,
                    ISource
            };
        }

        public string DeviceIdentifier { get; private set; }

        public double StartVoltage { get; set; }

        public double FinalVoltage { get; set; }

        public double IncrementVoltage { get; set; }

        public double CurrentLimit { get; set; }

        public double VoltageLimit { get; set; }

        public string Title => _keithley2400SerialDevice.Title;

        public string Description => _keithley2400SerialDevice.Description;

        public ISerialPort SerialPort => _keithley2400SerialDevice.SerialPort;

        public bool DeviceAvailable => _keithley2400SerialDevice.DeviceAvailable;

        public void Initialize(KEITHLEY_CONFIG keithleyConfig)
        {
            _configurations[(int)keithleyConfig]();
        }

        private void VSource()
        {
            SerialSafeWrite("*RST");
            // Initialization of the K2410: see description for details
            SerialSafeWrite(":SENS:FUNC:CONC ON");
            SerialSafeWrite(":SOUR:FUNC VOLT");
            SerialSafeWrite(":SENS:FUNC:ON 'VOLT:DC','CURR:DC'");
            //SerialSafeWrite(":SOUR:CLE:AUTO OFF");
            SerialSafeWrite(":SENS:VOLT:PROT 40 ");
            SerialSafeWrite(":SENS:VOLT:RANGE:AUTO OFF");

            SerialSafeWrite(":FORM:ELEM VOLT, CURR, TIME");
            SerialSafeWrite(":TRAC:TST:FORM ABS");
            SerialSafeWrite(":SENS:CURR:NPLC 10");
            SerialSafeWrite(":SOUR:VOLT:PROT 40");

            // Switch on volt monitor
            SerialSafeWrite(":SOUR:VOLT:LEV 0.0");

            // Referenced from old I-V code 
            SerialSafeWrite(":SOUR:CLE:AUTO OFF");                //prevent current off after a reading
            SerialSafeWrite(":FORM:ELEM VOLT, CURR, TIME");      // specify output buffer elements
            SerialSafeWrite(":TRAC:TST:FORM ABS");                //time stamp is absolute w.r.t. 1st reading
            SerialSafeWrite(":SYST:TIME:RES");                    //reset internal clock

            // Settings to be further specified by user
            SerialSafeWrite(":SENS:VOLT:NPLC 10");                //integration time in power-line cycles
            SerialSafeWrite(":SOUR:CURR:RANG:AUTO OFF");
            SerialSafeWrite($":SENS:CURR:PROT {CurrentLimit} ");            //fix current source range to 50mA


            SerialSafeWrite(":SYST:RSEN ON");   //Enable 4 wire sensing
                                                                          //globals.KT24Port.WriteLine(":FORM:ELEM?");
                                                                          // globals.FieldList += globals.KT24Port.ReadLine();
            SerialSafeWrite(":OUTP ON");   // Enables the output but its at zero volts at this point

        }

        private void ISource()
        {

            SerialSafeWrite("*RST");
            // Initialization of the K2410: see description for details
            SerialSafeWrite(":SENS:FUNC:CONC ON");
            SerialSafeWrite(":SOUR:FUNC CURR");
            SerialSafeWrite(":SENS:FUNC:ON 'VOLT:DC','CURR:DC'");
            SerialSafeWrite(":SENS:VOLT:PROT 20 ");
            SerialSafeWrite(":SENS:VOLT:RANGE:AUTO ON");
            SerialSafeWrite(":TRAC:FEED SENS");                  // Store readings in the buffer
            SerialSafeWrite(":SOUR:CURR:LEV 0");
            SerialSafeWrite(":SOUR:CLE:AUTO OFF");               //prevent current off after a reading
            SerialSafeWrite(":FORM:ELEM VOLT, CURR ");           // specify output buffer elements
            SerialSafeWrite(":SENS:VOLT:NPLC 5");                //integration time in power-line cycles

            SerialSafeWrite(String.Format($":SENS:VOLT:PROT {VoltageLimit}"));


            //SerialSafeWrite(":SYST:RSEN OFF");   //Enable 4 wire sensing
            // globals.KT24Port.WriteLine(":FORM:ELEM?");
            //globals.FieldList += globals.KT24Port.ReadLine();
            SerialSafeWrite(":OUTP ON");   // Enables the output but its at zero volts at this point
        }

        public void SerialSafeWrite(string data)
        {
            _keithley2400SerialDevice.SerialSafeWrite(data);
        }

        public string SerialSafeRead (string request)
        {
            return _keithley2400SerialDevice.SerialSafeRead(request);
        }

        public void ClearBuffers()
        {
            _keithley2400SerialDevice.ClearBuffers();
        }

        public void InitializeDevice(ISerialPortManager serialPortManager)
        {
            ((ISerialDevice)_keithley2400SerialDevice).InitializeDevice(serialPortManager);
        }

        public void ReleaseDevice()
        {
            ((ISerialDevice)_keithley2400SerialDevice).ReleaseDevice();
        }

        public void Dispose()
        {
            ((ISerialDevice)_keithley2400SerialDevice).Dispose();
        }
    }
}
