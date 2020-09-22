using Prism.Mvvm;
using Prism.Commands;
using System;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using C_V_App.CollectionExtensions;
using C_V_App.SerialPortWrappers;
using C_V_App.Models;
using C_V_App.Views;

namespace C_V_App.ViewModels
{
    public class CVTesterViewModel : BindableBase
    {
        private const String NEWLINE = "\n";
        private CVScan _cvScan;
        private CVTester _cvtester;
        private CVConfiguration _cvConfiguration;
        private CancellationTokenSource _cancellationTokenSource;

        private StringBuilder _monitorStringBuilder;
        private string _monitorText;
        private string _resultsFilename;
        private string _description;
        private string _statusMessage;
        private double _newFrequency;
        private bool _shortCircuitActive;
        private bool _openCircuitActive;
        private bool _devicesDiscovered;
        private bool _executing;
        private bool _emulate;

        private SerialPortManagerSelector _serialPortManagerSelector;
        private string _keithleyEmulationFile;
        private string _wayneKerrEmulationFile;
        private string _configurationFile;

        private ICVEnvironment _cvEnv;
        private IWayneKerr4300Model _wayneKerr;
        private IKeithley2400Model _keithley;


        public CVTesterViewModel (IKeithley2400ViewModel keithley2400ViewModel, 
            IWayneKerr4300ViewModel wayneKerr4300ViewModel, 
            CVTester cvTester)
        {
            _cvScan = new CVScan();
            _cvtester = cvTester;
            _cvtester.Closing += CloseProgram;
            Keithley = keithley2400ViewModel;
            WayneKerr = wayneKerr4300ViewModel;
            _wayneKerr = WayneKerr.GetModel(); 
            _keithley = Keithley.GetModel(); 

            _serialPortManagerSelector = new SerialPortManagerSelector(new SerialPortManager(),
                                                            new SerialPortEmulatorManager());

            _cvConfiguration = new CVConfiguration();
            _cvScan.Monitor = MonitorMessageDelegate;
            _monitorStringBuilder = new StringBuilder();
            FrequencyList = new ObservableCollection<double>();
            _shortCircuitActive = false;
            _openCircuitActive = false;
            _devicesDiscovered = false;
            Executing = false;
            Emulate = false; // SD++


            ReadConfigCommand = new DelegateCommand(ExecuteReadConfig, CanExecuteReadConfig)
                .ObservesProperty<string>(() => ConfigurationFile);

            SaveConfigCommand = new DelegateCommand(ExecuteSaveConfig, CanExecuteSaveConfig)
               .ObservesProperty<string>(() => ConfigurationFile);

            AddFrequencyCommand = new DelegateCommand(ExecuteAddFrequency, CanExecuteAddFrequency).ObservesProperty(() => NewFrequency);

            DeleteSelectedFrequenciesCommand = new DelegateCommand<ICollection<object>>(ExecuteDeleteSelectedFrequencies, CanExecuteDeleteSelectedFrequencies);

            DiscoveryCommand = new DelegateCommand(ExecuteDiscovery, CanExecuteDiscovery)
                 .ObservesProperty<bool>(() => Emulate)
                 .ObservesProperty<string>(() => KeithleyEmulationFile)
                 .ObservesProperty<string>(() => WayneKerrEmulationFile);

            ShortCircuitTestCommand = new DelegateCommand(ExecuteShortCircuitTest, CanExecuteShortCircuitTest)
                 .ObservesProperty<bool>(() => Emulate)
                 .ObservesProperty<bool>(() => Executing)
                 .ObservesProperty<bool>(() => ShortCircuitActive)
                 .ObservesProperty<bool>(() => DevicesDiscovered)
                 .ObservesProperty<string>(() => ResultsFileName);


            OpenCircuitTestCommand = new DelegateCommand(ExecuteOpenCircuitTest, CanExecuteOpenCircuitTest)
                 .ObservesProperty<bool>(() => Emulate)
                 .ObservesProperty<bool>(() => Executing)
                 .ObservesProperty<bool>(() => DevicesDiscovered)
                 .ObservesProperty<bool>(() => OpenCircuitActive)
                 .ObservesProperty<string>(() => ResultsFileName);


            CVTestCommand = new DelegateCommand(ExecuteCVTest, CanExecuteCVTest)
                 .ObservesProperty<bool>(() => Emulate)
                 .ObservesProperty<bool>(() => Executing)
                 .ObservesProperty<bool>(() => DevicesDiscovered)
                 .ObservesProperty<string>(() => ResultsFileName);

            StopCVTestCommand = new DelegateCommand(StopCVTest, CanExecuteStopCVTest)
                 .ObservesProperty<bool>(() => Executing);

        }

        // Setup Properties
        // ================
        public bool Emulate
        {
            get { return _emulate; }
            set { SetProperty<bool>(ref _emulate, value); }
        }
        public bool Executing
        {
            get { return _executing; }
            set { SetProperty<bool>(ref _executing, value); }
        }

        public bool DevicesDiscovered
        {
            get { return _devicesDiscovered; }
            set { SetProperty<bool>(ref _devicesDiscovered, value); }
        }

        public bool OpenCircuitActive
        {
            get { return _openCircuitActive; }
            set { SetProperty<bool>(ref _openCircuitActive, value); }
        }

        public bool ShortCircuitActive
        {
            get { return _shortCircuitActive; }
            set { SetProperty<bool>(ref _shortCircuitActive, value); }
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set { SetProperty<string>(ref _statusMessage, value); }
        }

        public string KeithleyEmulationFile
        {
            get { return _keithleyEmulationFile; }
            set { SetProperty<string>(ref _keithleyEmulationFile, value); }
        }

        public string WayneKerrEmulationFile
        {
            get { return _wayneKerrEmulationFile; }
            set { SetProperty<string>(ref _wayneKerrEmulationFile, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty<string>(ref _description, value); }
        }
        public string MonitorText
        {
            get { return _monitorText; }
            set { SetProperty<string>(ref _monitorText, value); }
        }

        public IKeithley2400ViewModel Keithley { get; }

        public IWayneKerr4300ViewModel WayneKerr { get; }

        //Discovery Properties
        //====================
        public string KeithleyPort
        {
            get { return Keithley.PortName; }
            set { Keithley.PortName = value; }
        }

        public string WayneKerrPort
        {
            get { return WayneKerr.PortName; }
            set { WayneKerr.PortName = value; }
        }


        // Configuration Properties
        // ========================
        public string ConfigurationFile
        {
            get { return _configurationFile; }
            set { SetProperty<string>(ref _configurationFile, value); }
        }

        public double StartVoltage
        {
            get { return Keithley.StartVoltage; }
            set { Keithley.StartVoltage = value; }
        }

        public double FinalVoltage
        {
            get { return Keithley.FinalVoltage; }
            set { Keithley.FinalVoltage = value; }
        }

        public double CurrentLimit
        {
            get { return Keithley.CurrentLimit; }
            set { Keithley.CurrentLimit = value; }
        }

        public double IncrementVoltage
        {
            get { return Keithley.IncrementVoltage; }
            set { Keithley.IncrementVoltage = value; }
        }

        public double Amplitude
        {
            get { return WayneKerr.Amplitude; }
            set { WayneKerr.Amplitude = value; }
        }

        public double NewFrequency
        {
            get { return _newFrequency; }
            set { SetProperty<double>(ref _newFrequency, value);  }
        }

        public ObservableCollection<double> FrequencyList { get; }

        public string ResultsFileName
        {
            get { return _resultsFilename; }
            set { SetProperty<string>(ref _resultsFilename, value); }
        }



        // Commands
        // =========
        public ICommand DiscoveryCommand { get; set; }

        public bool CanExecuteDiscovery()
        {
            return !Emulate || 
                (!String.IsNullOrWhiteSpace(KeithleyEmulationFile) && !String.IsNullOrWhiteSpace(WayneKerrEmulationFile));
        }

        public void ExecuteDiscovery()
        {

            DevicesDiscovered = true;
            StatusMessage = "Discovering Devices";

            _cvEnv = new CVEnvironment(KeithleyEmulationFile, WayneKerrEmulationFile, Emulate);
            var serialPortManager = _serialPortManagerSelector.GetSerialManager(_cvEnv);

            if (_wayneKerr.SerialPort != null && _wayneKerr.SerialPort.IsOpen)
            {
                _wayneKerr.SerialPort.Close();
            }
            if (_keithley.SerialPort != null && _keithley.SerialPort.IsOpen)
            {
                _keithley.SerialPort.Close();
            }

            try
            {
                _keithley.InitializeDevice(serialPortManager);
            }
            catch (Exception ex)
            {
                ExceptionMessageBoxDelegate(ex.Message);
            }

            try
            {
                _wayneKerr.InitializeDevice(serialPortManager);
            }
            catch (Exception ex)
            {
                ExceptionMessageBoxDelegate(ex.Message);
            }
            if (_wayneKerr.SerialPort == null)
            {
                DevicesDiscovered = false;
                StatusMessage = "Wayne Kerr device not found";
            }
            else
            {
                WayneKerr.PortName = _wayneKerr.SerialPort.PortName;
                _wayneKerr.SerialPort.Close();
            }
            if (_keithley.SerialPort == null)
            {
                DevicesDiscovered = false;
                StatusMessage = "Keithley device not found";
            }
            else
            {
                Keithley.PortName = _keithley.SerialPort.PortName;
                _keithley.SerialPort.Close();
            }
            if (DevicesDiscovered)
            {
                StatusMessage = "Devices sucessfully discovered";
            }
        }

        public ICommand ReadConfigCommand { get; set; }

        public bool CanExecuteReadConfig()
        {
            return !String.IsNullOrWhiteSpace(ConfigurationFile); ;
        }

        public void ExecuteReadConfig()
        {
            try
            {
                _cvConfiguration.LoadConfiguration(ConfigurationFile);
                StartVoltage = _cvConfiguration.StartVoltage;
                FinalVoltage = _cvConfiguration.FinalVoltage;
                IncrementVoltage = _cvConfiguration.IncrementVoltage;
                CurrentLimit = _cvConfiguration.CurrentLimit;
                Amplitude = _cvConfiguration.Amplitude;
                FrequencyList.Clear();
                foreach (var frequency in _cvConfiguration.TestWaveFrequency)
                {
                    FrequencyList.Add(frequency);
                }
                StatusMessage = $"Configuration File {ConfigurationFile} loaded";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error {ex.Message} reading Configuration File: {ConfigurationFile}";
            }

        }
        public ICommand SaveConfigCommand { get; set; }

        public bool CanExecuteSaveConfig()
        {
            return !String.IsNullOrWhiteSpace(ConfigurationFile);
        }

        public void ExecuteSaveConfig()
        {
            try
            {
                _cvConfiguration.StartVoltage = StartVoltage;
                _cvConfiguration.FinalVoltage = FinalVoltage;
                _cvConfiguration.IncrementVoltage = IncrementVoltage;
                _cvConfiguration.CurrentLimit = CurrentLimit;
                _cvConfiguration.Amplitude = Amplitude;
                _cvConfiguration.TestWaveFrequency.Clear();
                foreach (var frequency in FrequencyList)
                {
                    _cvConfiguration.TestWaveFrequency.Add(frequency);
                }

                _cvConfiguration.SaveConfiguration(ConfigurationFile);
                StatusMessage = $"Configuration File {ConfigurationFile} saved";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error {ex.Message} saving Configuration File: {ConfigurationFile}";
            }

        }
        public ICommand AddFrequencyCommand { get; set; }

        public bool CanExecuteAddFrequency()
        {
            return NewFrequency > 0.0 && !FrequencyList.Contains(NewFrequency);
        }

        public void ExecuteAddFrequency()
        {
            FrequencyList.Add(NewFrequency);
            FrequencyList.Sort();
        }

        public ICommand DeleteSelectedFrequenciesCommand { get; set; }

        public bool CanExecuteDeleteSelectedFrequencies(ICollection<object> selectedItems)
        {
            return true;
        }

        public void ExecuteDeleteSelectedFrequencies(ICollection<object> selectedItems)
        {
            IList<object> copy = selectedItems.ToList<object>();

            foreach (var item in copy)
            {
                FrequencyList.Remove((double)item);
            }
        }

        public ICommand ShortCircuitTestCommand { get; set; }

        public bool CanExecuteShortCircuitTest()
        {
            return DevicesDiscovered && !Executing && !_shortCircuitActive;
        }

        public void ExecuteShortCircuitTest()
        {
            ShortCircuitActive = true;
            
            WayneKerr.GetModel().SerialSafeWrite(":CAL:OC-TRIM 3");
        }
        public ICommand OpenCircuitTestCommand { get; set; }

        public bool CanExecuteOpenCircuitTest()
        {
            return DevicesDiscovered && !Executing && !_openCircuitActive;
        }

        public void ExecuteOpenCircuitTest()
        {
            OpenCircuitActive = true;
             WayneKerr.GetModel().SerialSafeWrite(":CAL:OC-TRIM 3");
        }

        public ICommand CVTestCommand { get; set; }

        public bool CanExecuteCVTest()
        {
            bool resultsFilePopulated = !String.IsNullOrWhiteSpace(ResultsFileName);

            return !Executing &&
                resultsFilePopulated &&
                DevicesDiscovered;
        }

        public void ExecuteCVTest()
        {
            // Open Results file for writing
            try
            {
                _cvScan.ResultsStream = new StreamWriter(ResultsFileName);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Unable to open {ResultsFileName} for writing: {ex.Message}";
                return;
            }
            _cvScan.Description = Description;
            _cvScan.Frequencies = FrequencyList;
            _monitorStringBuilder.Clear();
            Keithley.SetModelForExecution();
            WayneKerr.SetModelForExecution();


            Executing = true;
            MonitorText = "";
            StatusMessage = "Executing CV test";

            if (!_keithley.SerialPort.IsOpen)
            {
                _keithley.SerialPort.Open();
            }
            if (!_wayneKerr.SerialPort.IsOpen)
            {
                _wayneKerr.SerialPort.Open();
            }
            _cancellationTokenSource = new CancellationTokenSource();
            Task task = Task.Factory.StartNew(() => _cvScan.Multiscan(_keithley, _wayneKerr, _cvEnv, _cancellationTokenSource.Token),
                _cancellationTokenSource.Token).ContinueWith((parentTask) =>
                {
                    EndExecution();
                    StatusMessageDelegate("End of C-V Program session");
                }).ContinueWith((parentTask) =>
                {
                    new ErrorHandlerViewModel(parentTask, ExceptionMessageBoxDelegate);
                    EndExecution();
                    StatusMessageDelegate("C-V Program failed");
                }, TaskContinuationOptions.OnlyOnFaulted);

        }
        public ICommand StopCVTestCommand { get; set; }

        public bool CanExecuteStopCVTest()
        {
            return Executing;
        }

        public void StopCVTest()
        {
            _cancellationTokenSource.Cancel();
        }

        // Update Monitor text from Thread.
        private void MonitorMessageDelegate(string message)
        {
            _cvtester.Dispatcher.Invoke(() =>
            {
                _monitorStringBuilder.Append(message);
                _monitorStringBuilder.Append(NEWLINE);
                MonitorText = _monitorStringBuilder.ToString();
            });
        }

        // Update Status text from Thread.
        private void StatusMessageDelegate(string message)
        {
            _cvtester.Dispatcher.Invoke(() =>
            {
                StatusMessage = message;
            });
        }

        // Exception Message Box.
        private void ExceptionMessageBoxDelegate (string message)
        {

            _cvtester.Dispatcher.Invoke(() =>
            {
                var result = MessageBox.Show(message);
            });
        }

        public void EndExecution()
        {
            if (Executing)
            {
                 Executing = false;
                _keithley.SerialPort.Close();
                _wayneKerr.SerialPort.Close();
                _cvScan.ResultsStream.Close();
            }
        }

        private void ReleaseDevices()
        {
            _keithley.ReleaseDevice();
            _wayneKerr.ReleaseDevice();
        }

        private void CloseProgram(object sender, CancelEventArgs args)
        {
            EndExecution();
            ReleaseDevices();
        }
    }
}
