using System.Collections.Generic;
using System.IO.Ports;

namespace C_V_App.SerialPortWrappers
{
    public interface ISerialPort
    {
        int BaudRate { get; set; }
        int DataBits { get; set; }
        int ReadTimeout { get; set; }
        int WriteTimeout { get; set; }
        int ReadBufferSize { get; set; }
        int WriteBufferSize { get; set; }
        bool DtrEnable { get; set; }
        bool RtsEnable { get; set; }
        bool IsOpen { get; }
        string PortName { get; set; }
        string NewLine { get; set; }
        Parity Parity { get; set; }
        StopBits StopBits { get; set; }
        Handshake Handshake { get; set; }

        void Open();

        void Close();

        string ReadLine();

        string ReadExisting();

        void WriteLine(string line);

        void DiscardInBuffer();

        void DiscardOutBuffer();
    }
}
