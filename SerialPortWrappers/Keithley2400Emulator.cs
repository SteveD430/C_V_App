using System;
using System.IO;
using System.Collections.Generic;
using C_V_App.StringExtentions;
using C_V_App.Exceptions;

namespace C_V_App.SerialPortWrappers
{ 
    [System.ComponentModel.DesignerCategory("Code")]
    public class Keithley2400Emulator : WrappedSerialPort, ISerialPortEmulator
    {
        private SerialPortStates _state;
        private string _emulatorFileName;
        private StreamWriter _debug;

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
                {"SYSTem:RSENse?", Sensors }
            };
            _debug = new StreamWriter(@"D:\Steve\Projects\Microfab\C-V\C_V_App\kt_debug.dat");
            _readLineResponse = null;
            _state = SerialPortStates.Closed;

            _openDelegates = new VoidZero[] { AlreadyOpen, AlreadyOpen, Closed2Open };
            _closeDelegates = new VoidZero[] { Open2Closed, Open2Closed, NotOpen };
            _nullActionDelegates = new VoidZero[] { NullAction, NullAction, NotOpen };

            _writeLineDelegates = new VoidString[] { WriteLineDelegate, WriteLineDelegate, NotOpen };

            _readLineDelegates = new StringZero[] { OpenReadLine, InitializedReadLine, NotOpenReadLine };
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

        // ISerialPort
        // ===========
        public new bool IsOpen => _state != SerialPortStates.Closed;

        public new void Open()
        {
            _debug.WriteLine("Open()");
            _openDelegates[(int)_state](nameof(Open));
        }

        public new void Close()
        {
            _debug.WriteLine("Close()");
            _closeDelegates[(int)_state](nameof(Close));
        }

        public new void DiscardInBuffer()
        {
            _debug.WriteLine("DiscardInBuffer()");
            _nullActionDelegates[(int)_state](nameof(DiscardInBuffer));
        }

        public new void DiscardOutBuffer()
        {
            _debug.WriteLine("DiscardOutBuffer()");
            _nullActionDelegates[(int)_state](nameof(DiscardOutBuffer));

        }

        public new void WriteLine(string line)
        {
            _debug.WriteLine(line);
            _writeLineDelegates[(int)_state](nameof(WriteLine), line);
        }

        public new string ReadExisting()
        {
            _debug.WriteLine("ReadExisting()");
            return _readLineDelegates[(int)_state](nameof(ReadExisting));
        }

        public new string ReadLine()
        {
            _debug.WriteLine("ReadLine()");
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
            throw new System.Exception($"{DEVICE_NAME}: Attempt to reopen already open port in function {functionName}");
        }

        private void Closed2Open(string functionName)
        {
            _readLineResponse = KEITHLEY_2400;
            _state = SerialPortStates.Open;
        }

        private void NotOpen(string functionName)
        {
            throw new System.Exception($"{DEVICE_NAME}: Attempt to access port before opening in function {functionName}");
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
            throw new System.Exception($"{DEVICE_NAME}: Attempt to access port before opening in function {functionName}");
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
        private string InitializedReadLine(string functionname)
        {
            return "Initialized";
        }
        private string NotOpenReadLine(string functionName)
        {
            throw new System.Exception($"{DEVICE_NAME}: Attempt to read from port before opening in function {functionName}");
        }
        #endregion StringZero
    }
}
