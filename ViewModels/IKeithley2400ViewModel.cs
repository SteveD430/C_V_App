using System;
using C_V_App.Models;

namespace C_V_App.ViewModels
{
    public interface IKeithley2400ViewModel
    {
        string PortName { get; set; }

        double StartVoltage { get; set; }

        double FinalVoltage { get; set; }

        double IncrementVoltage { get; set; }

        double CurrentLimit { get; set; }

        IKeithley2400Model GetModel();

        void SetModelForExecution();

    }
}
