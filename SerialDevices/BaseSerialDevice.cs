using System;
using System.Threading;
using C_V_App.Exceptions;
using C_V_App.SerialPortWrappers;

namespace C_V_App.SerialDevices
{
    public abstract class BaseSerialDevice : ISerialDevice
    {
        protected const string NEWLINE = "\n";

        #region ISerialDevice
        public abstract string Title { get; }

        public abstract string Description { get; }

        public abstract ISerialPort SerialPort { get; protected set; }

        public bool DeviceAvailable
        {
            get { return SerialPort != null; }
        }

        public abstract void InitializeDevice(ISerialPortManager serialPortManager);

        public virtual void ReleaseDevice()
        {
            if (SerialPort != null && SerialPort.IsOpen)
            {
                SerialPort.Close();
            }
            SerialPort = null;
        }
        #endregion ISerialDevice

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only when Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BaseSerialDevice() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line when the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        public void SerialSafeWriteWithDelay(string send)
        {
            SerialSafeWrite(send);
            Thread.Sleep(50);
        }

        public void SerialSafeWrite(string send)
        {
            if (SerialPort.IsOpen)
            {
                try
                {
                    SerialPort.WriteLine(send);
                }
                catch (Exception RWx)
                {
                    throw new PortCommunicationException($"Unable to Write {send} to {Description} - Error: {RWx.Message}", RWx);
                }
            }
            else
            {
                throw new PortCommunicationException($"Unable to send to {Description} - COM port unavailable");
            }
        }

        public string SerialSafeRead(string request)
        {
            string response = null;
            SerialSafeWrite(request);

            if (SerialPort.IsOpen)
            {
                try
                {
                    response = SerialPort.ReadLine();
                }
                catch (Exception RWx)
                {
                    throw new PortCommunicationException($"Failure to read result of {request} request from {Description} - Error: {RWx.Message}", RWx);
                }
            }
            else
            {
                throw new PortCommunicationException($"Unable to request {request} from {Description} - COM port unavailable");
            }
            return response;
        }

        public void ClearBuffers()
        {
            SerialPort.DiscardInBuffer();
            SerialPort.DiscardOutBuffer();
        }
    }
}
