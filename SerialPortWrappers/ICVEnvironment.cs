using System;


namespace C_V_App.SerialPortWrappers
{
    public interface ICVEnvironment
    {
        bool Emulate { get; }

        string KeithleyEmulationFilename { get; }

        string WayneKerrEmulationFilename { get; }
    }
}
