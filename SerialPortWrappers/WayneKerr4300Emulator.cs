﻿using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Ports;
using C_V_App.Exceptions;

namespace C_V_App.SerialPortWrappers
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class WayneKerr4300Emulator : WrappedEmulatorSerialPort, ISerialPortEmulator
    {
        private SerialPortStates _state;
        private string _emulatorFileName;

        private delegate void VoidZero(string functionName);
        private delegate void VoidString(string functionName, string arg);
        private delegate string StringZero(string functionName);

        private delegate string CommandResponse();

        private Dictionary<string, CommandResponse> _commandResponses;
        private string _readLineResponse;
        private const string WAYNE_KERR_4300 = "WAYNE KERR 4300";
        private const string DEVICE_NAME = WAYNE_KERR_4300;
        private string _level = "2";
        private string _freq = "500Khz";
        private const string FUNC1 = "9";
        private const string FUNC2 = "7";

        public WayneKerr4300Emulator(string name) : base(name)
        {
#if DEBUG
            Debug = new StreamWriter(@"D:\Steve\Projects\Microfab\C-V\C_V_App\wkDebug.dat");
#else
            var memStream = new MemoryStream(100);
            Debug = new StreamWriter(memStream);
#endif

            _commandResponses = new Dictionary<string, CommandResponse>()
            {
                {"*IDN?", DeviceName },
                {":READ?", DataPoint },
                {":TRIG", DataPoint},
                {":MEAS:LEV?", ReturnLevel },
                {":MEAS:FREQ?", ReturnFreq },
                {":MEAS:FUNC1?", ReturnFunc1 },
                {":MEAS:FUNC2?", ReturnFunc2 }
            };

            _readLineResponse = null;
            _state = SerialPortStates.Closed;

            _openDelegates = new VoidZero[] { AlreadyOpen, Closed2Open };
            _closeDelegates = new VoidZero[] { Open2Closed,  NotOpen };
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
                    throw new EmulationFileNotFoundException($"Error opening Wayne Kerr Emulation File {_emulatorFileName}", ex);
                }
            }
        }
        public StreamReader EmulationDataStream { get; set; }

        // ISerialPort
        // ===========
 
        public new bool IsOpen => _state == SerialPortStates.Open;

        public new void Open()
        {
            Debug.WriteLine("Open()");
            _openDelegates[(int)_state](nameof(Open));
        }

        public new void Close()
        {
            Debug.WriteLine("Close()");
            _closeDelegates[(int)_state](nameof(Close));
        }

        public new void DiscardInBuffer()
        {
            Debug.WriteLine("DiscardInBuffer()");
            _nullActionDelegates[(int)_state](nameof(DiscardInBuffer));
        }

        public new void DiscardOutBuffer()
        {
            Debug.WriteLine("DiscardOutBuffer()");
            _nullActionDelegates[(int)_state](nameof(DiscardOutBuffer));

        }

        public new void WriteLine(string line)
        {
            Debug.WriteLine(line);
            _writeLineDelegates[(int)_state](nameof(WriteLine), line);
        }

        public new string ReadExisting()
        {
            Debug.WriteLine("ReadExisting()");
            return _readLineDelegates[(int)_state](nameof(ReadExisting));
        }

        public new string ReadLine()
        {
            Debug.WriteLine("ReadLine()");
            return _readLineDelegates[(int)_state](nameof(ReadLine));
        }

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
            _readLineResponse = WAYNE_KERR_4300;
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

           if ( _commandResponses.TryGetValue(arg, out responseDelgate))
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
            throw new System.Exception($"{DEVICE_NAME}: Attempt to read from port before opening in function {functionName}");
        }
#endregion StringZero
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
                    throw new EmulationFileReadException($"Wayne Kerr Emulation data stream not defined");
                }
            try
            {
                dataPoint = EmulationDataStream.ReadLine();
            }
            catch (Exception ex)
            {
                throw new EmulationFileReadException($"Error reading Wayne Kerr Emualtion data stream", ex);
            }
            return dataPoint;
        }

        public string ReturnLevel()
        {
            return _level;
        }
        public string ReturnFreq()
        {
            return _freq;
        }
        public string ReturnFunc1()
        {
            return FUNC1;
        }
        public string ReturnFunc2()
        {
            return FUNC2;
        }
        #endregion Command Response
    }
}
