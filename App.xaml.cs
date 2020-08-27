using System.Windows;
using C_V_App.Models;
using C_V_App.ViewModels;
using C_V_App.Views;
using C_V_App.SerialPortWrappers;


namespace C_V_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            CVTester cvTester = new CVTester();

            IKeithley2400Model kt2400 = new Keithley2400Model();
            IWayneKerr4300Model wk4300 = new WayneKerr4300Model();

            IKeithley2400ViewModel kt2400vm = new Keithley2400ViewModel(kt2400);
            IWayneKerr4300ViewModel wk4300vm = new WayneKerr4300ViewModel(wk4300);

            CVTesterViewModel vm = new CVTesterViewModel(kt2400vm, wk4300vm, cvTester);
            cvTester.DataContext = vm;
            cvTester.Show();
        }
    }
}
