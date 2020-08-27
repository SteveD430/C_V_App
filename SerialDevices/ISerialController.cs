
namespace C_V_App.SerialDevices
{
    public interface ISerialController : ISerialDevice
    {
        void RequestData();

        string ReadData();
    }
}
