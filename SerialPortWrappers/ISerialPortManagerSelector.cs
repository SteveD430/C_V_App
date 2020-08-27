
namespace C_V_App.SerialPortWrappers
{
    public interface ISerialPortManagerSelector
    {
        ISerialPortManager GetSerialManager(ICVEnvironment cvEnv);
    }
}
