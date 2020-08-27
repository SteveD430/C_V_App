using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C_V_App.Exceptions;

namespace C_V_App.SerialPortWrappers
{
    public class WanyeKerr4300V2Emulator : WrappedSerialPort, ISerialPortEmulator
    {
        private string _ese = "";
        private string _sre = "";

        private delegate void CommonCommandActionDelegate(string arg);
        private delegate string CommonCommandFuncDelegate(string arg);

        private delegate void SubSystemCommandActionDelegate(string arg);
        private delegate string SubSystemCommandFuncDelegate(string arg);


        private SerialPortStates _state;
        private string _emulatorFileName;

        private Dictionary<string, CommonCommandActionDelegate> _commonActionCommands;
        private Dictionary<string, CommonCommandFuncDelegate> _commonFuncCommands;

        private Dictionary<string, SubSystemCommandActionDelegate> _subSystemActionCommands;
        private Dictionary<string, SubSystemCommandFuncDelegate> _subSysyemFuncCommands;


        public WanyeKerr4300V2Emulator(string name) : base (name)
        {

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
        #region CommonActionCommands
        private void CLS(string arg)
        {

        }

        private void ESE(string arg)
        {
            _ese = arg;
        }

        private void SRE(string arg)
        {

        }

        private void RST(string arg)
        {

        }

        private void TRG(string arg)
        {

        }

        private void OPC(string arg)
        {

        }

        private void WAI(string arg)
        {

        }
        #endregion CommonActionCommands

        #region CommonFuncCommands
        private string ESEquery(string arg)
        {
            return _ese;
        }

        private string ESRquery(string arg)
        {
            return "0";
        }

        private string SREquery(string arg)
        {
            return _sre;
        }

        private string STBquery(string arg)
        {
            return "0";
        }

        private string IDNquery(string arg)
        {
            return "WAYNE KERR 4300";
        }

        private string OPTquery(string arg)
        {
            return "4300";
        }

        private string OPCquery(string arg)
        {
            return "1";
        }
        #endregion CommonFuncCommands
        #region SubSystemAcionCommands
        private void MEAS_FREQ(string arg)
        {

        }

        private void MEAS_LEV(string arg)
        {

        }

        private void MEAS_SPEED(string arg)
        {

        }

        private void MEAS_RANGE(string arg)
        {

        }

        private void MEAS_EQU_CCT(string arg)
        {

        }

        private void MEAS_FUNC(string arg)
        {

        }

        private void CAL_OC(string arg)
        {

        }

        private void CAL_SC(string arg)
        {

        }

        private void MEAS_TRIG(string arg)
        {

        }

        private void TRIG(string arg)
        {

        }
        #endregion SubSystemActionCommands
        #region SubSystemFuncCommands
        #endregion SubSystemFuncCommands
    }
}
