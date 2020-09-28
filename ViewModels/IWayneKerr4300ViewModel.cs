using System;
using C_V_App.Models;

namespace C_V_App.ViewModels
{
    public interface IWayneKerr4300ViewModel
    {
        string PortName { get; set; }

        double Amplitude { get; set; }

        IWayneKerr4300Model GetModel();

        void ReleaseDevice();

        void SetModelForExecution();
    }
}
