using System;
using C_V_App.SerialPortWrappers;

namespace C_V_App.SerialDevices
{
    public interface ISerialDevice : IDisposable
    {
        string Title { get; }

        String Description { get; }

        ISerialPort SerialPort { get; }

        bool DeviceAvailable { get; }

        string SerialSafeRead(string request);

        void SerialSafeWrite(string data);

        void SerialSafeWriteWithDelay(string data);

        void ClearBuffers();

        void InitializeDevice(ISerialPortManager serialPortManager);

        void ReleaseDevice();
    }
}
