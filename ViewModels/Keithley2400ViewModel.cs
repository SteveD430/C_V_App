using C_V_App.Models;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace C_V_App.ViewModels
{
    public class Keithley2400ViewModel : BindableBase, IKeithley2400ViewModel
    {
        private string _portName;
        private double _startVoltage;
        private double _finalVoltage;
        private double _currentLimit;
        private double _incrementVoltage;

        private IKeithley2400Model _keithley2400;

        public Keithley2400ViewModel(IKeithley2400Model keithley2400)
        {
            _keithley2400 = keithley2400;

            _startVoltage = 0.0;
            _finalVoltage = 0.0;
            _currentLimit = 0.0;
            _incrementVoltage = 0.0;
        }

        public string PortName
        {
            get { return _portName; }
            set { SetProperty<string>(ref _portName, value); }
        }

        public double StartVoltage
        {
            get { return _startVoltage; }
            set { SetProperty<double>(ref _startVoltage, value); }
        }

        public double FinalVoltage
        {
            get { return _finalVoltage; }
            set { SetProperty<double>(ref _finalVoltage, value); }
        }

        public double IncrementVoltage
        {
            get { return _incrementVoltage; }
            set { SetProperty<double>(ref _incrementVoltage, value); }
        }

        public double CurrentLimit
        {
            get { return _currentLimit; }
            set { SetProperty<double>(ref _currentLimit, value); }
        }

        public IKeithley2400Model GetModel()
        {
            return _keithley2400;
        }

        public void SetModelForExecution()
        {
            _keithley2400.StartVoltage = StartVoltage;
            _keithley2400.FinalVoltage = FinalVoltage;
            _keithley2400.IncrementVoltage = IncrementVoltage;
            _keithley2400.CurrentLimit = CurrentLimit;
        }
    }
}
