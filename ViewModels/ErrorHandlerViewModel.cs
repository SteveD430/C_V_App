using System.Text;
using System.Threading.Tasks;
using C_V_App.Models;

namespace C_V_App.ViewModels
{
    public class ErrorHandlerViewModel 
    {
        private string NEWLINE = "\n";
        public ErrorHandlerViewModel(Task faultedTask, CVMonitorDelegate Monitor)
        {
            var errorMessages = new StringBuilder();
            foreach (var ex in faultedTask.Exception.InnerExceptions)
            {
                errorMessages.Append(ex.Message);
                errorMessages.Append(NEWLINE);
            }
            Monitor(errorMessages.ToString());
        }
    }
}
