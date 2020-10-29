using System;
using System.IO;
using System.Collections.Generic;
using C_V_App.StringExtentions;
using C_V_App.Exceptions;

namespace C_V_App.SerialPortWrappers
{ 
    [System.ComponentModel.DesignerCategory("Code")]
    public class Keithley2400Emulator : WrappedEmulatorSerialPort, ISerialPortEmulator
    {
        private SerialPortStates _state;
        private string _emulatorFileName;

        private delegate void VoidZero(string functionName);
        private delegate void VoidString(string functionName, string arg);
        private delegate string StringZero(string functionName);

        private delegate string CommandResponse();

        private Dictionary<string, CommandResponse> _commandResponses;
        private string _readLineResponse;
        private const string KEITHLEY_2400 = "KEITHLEY 2400";
        private const string DEVICE_NAME = KEITHLEY_2400;

        public Keithley2400Emulator(string name) : base(name)
        {
            _commandResponses = new Dictionary<string, CommandResponse>()
            {
                {"*IDN?", DeviceName },
                {"READ?", DataPoint },
                {"SYSTem:RSENse?", Sensors },
                {":FORM:ELEM?", Headers }
            };
#if DEBUG
            Debug = new StreamWriter(@"D:\Steve\Projects\Microfab\C-V\C_V_App\kt_DEBUG.dat");
#else
            var memStream = new MemoryStream(100);
            Debug = new StreamWriter(memStream);
#endif

            _readLineResponse = null;
            _state = SerialPortStates.Closed;

            _openDelegates = new VoidZero[] { AlreadyOpen, Closed2Open };
            _closeDelegates = new VoidZero[] { Open2Closed, NotOpen };
            _nullActionDelegates = new VoidZero[] { NullAction, NotOpen };

            _writeLineDelegates = new VoidString[] { WriteLineDelegate, NotOpen };

            _readLineDelegates = new StringZero[] { OpenReadLine, NotOpenReadLine };
        }

        // ISerialPortEmulator
        // ===================
        public string EmulatorFileName
        {
            get { return _emulatorFileName; }
            set
            {
                _emulatorFileName = value;
                try
                {
                    EmulationDataStream = new StreamReader(_emulatorFileName);
                }
                catch (Exception ex)
                {
                    throw new EmulationFileNotFoundException($"Error opening Keithley Emulation File {_emulatorFileName}", ex);
                }
            }
        }

        public StreamReader EmulationDataStream { get; set; }

        public new bool IsOpen => _state == SerialPortStates.Open;

        public new void Open()
        {
            Debug.WriteLine("Open()");
            Debug.Flush();

            _openDelegates[(int)_state](nameof(Open));
        }

        public new void Close()
        {

            Debug.WriteLine("Close()");
            Debug.Flush();

            _closeDelegates[(int)_state](nameof(Close));
        }

        public new void DiscardInBuffer()
        {

            Debug.WriteLine("DiscardInBuffer()");
            Debug.Flush();

            _nullActionDelegates[(int)_state](nameof(DiscardInBuffer));
        }

        public new void DiscardOutBuffer()
        {
            Debug.WriteLine("DiscardOutBuffer()");
            Debug.Flush();
            _nullActionDelegates[(int)_state](nameof(DiscardOutBuffer));

        }

        public new void WriteLine(string line)
        {

            Debug.WriteLine(line);
            Debug.Flush();

            _writeLineDelegates[(int)_state](nameof(WriteLine), line);
        }

        public new string ReadExisting()
        {
            Debug.WriteLine("ReadExisting()");
            Debug.Flush();
            return _readLineDelegates[(int)_state](nameof(ReadExisting));
        }

        public new string ReadLine()
        {

            Debug.WriteLine("ReadLine()");

            return _readLineDelegates[(int)_state](nameof(ReadLine));
        }

#region Command Response
        public string DeviceName()
        {
            return DEVICE_NAME;
        }

        public string DataPoint()
        {
            string dataPoint = null;

            if (EmulationDataStream == null)
            {
                throw new EmulationFileReadException($"Keithley Emulation data stream not defined");
            }

            try
            {
                dataPoint = EmulationDataStream.ReadLine();
            }
            catch (Exception ex)
            {
                throw new EmulationFileReadException($"Error reading Keithley Emulation data stream", ex);
            }
 
            return dataPoint;
        }

        public string DiscardDataPoint()
        {
            return "0.0";
        }

        public string Sensors()
        {
            return "2";
        }

        public string Headers()
        {
            return "Volts, Amps, Time";
        }
#endregion Command Response

        private VoidZero[] _openDelegates;
        private VoidZero[] _closeDelegates;
        private VoidZero[] _nullActionDelegates;
        private VoidString[] _writeLineDelegates;
        private StringZero[] _readLineDelegates;
        //private VoidZero[] 

#region VoidZeroDelegates
        private void AlreadyOpen(string functionName)
        {
            throw new Exception($"{DEVICE_NAME}: Attempt to reopen already open port in function {functionName}");
        }

        private void Closed2Open(string functionName)
        {
            _readLineResponse = KEITHLEY_2400;
            _state = SerialPortStates.Open;
        }

        private void NotOpen(string functionName)
        {
            throw new Exception($"{DEVICE_NAME}: Attempt to access port before opening in function {functionName}");
        }

        private void Open2Closed(string functionName)
        {
            _state = SerialPortStates.Closed;
        }

        private void NullAction(string functionName)
        {

        }
#endregion VoidZeroDelegates
#region VoidStringDelegates
        private void NotOpen(string functionName, string arg)
        {
            throw new Exception($"{DEVICE_NAME}: Attempt to access port before opening in function {functionName}");
        }

        private void WriteLineDelegate(string functionName, string arg)
        {
            _readLineResponse = null;
            CommandResponse responseDelgate;
            if (_commandResponses.TryGetValue(arg.MaxLength(13), out responseDelgate))
            {
                _readLineResponse = responseDelgate();
            }
        }

#endregion VoidStringDelegates
#region StringZero
        private string OpenReadLine(string functionname)
        {
            return _readLineResponse;
        }

        private string NotOpenReadLine(string functionName)
        {
            throw new Exception($"{DEVICE_NAME}: Attempt to read from port before opening in function {functionName}");
        }
#endregion StringZero
    }
}
