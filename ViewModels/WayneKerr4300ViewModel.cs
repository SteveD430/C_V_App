using C_V_App.Models;
using Prism.Mvvm;

namespace C_V_App.ViewModels
{
    public class WayneKerr4300ViewModel : BindableBase, IWayneKerr4300ViewModel
    {
        private string _portName;
        private double _acAmplitude;

        private IWayneKerr4300Model _wayneKerr4300;

        public WayneKerr4300ViewModel(IWayneKerr4300Model wayneKerr4300)
        {
            _wayneKerr4300 = wayneKerr4300;
            _acAmplitude = 0.5;
        }

        public string PortName
        {
            get { return _portName; }
            set { SetProperty<string>(ref _portName, value); }
        }

        public double Amplitude
        {
            get { return _acAmplitude; }
            set { SetProperty<double>(ref _acAmplitude, value); }
        }

        public IWayneKerr4300Model GetModel()
        {
            return _wayneKerr4300;
        }

        public void SetModelForExecution()
        {
            _wayneKerr4300.Amplitude = Amplitude;
        }
    }
}
