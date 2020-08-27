using System;


namespace C_V_App.SerialPortWrappers
{
    public class CVEnvironment : ICVEnvironment
    {
        public CVEnvironment (string keithleyFilename, string wayneKerrFilename, bool emulate)
        {
            Emulate = emulate;
            KeithleyEmulationFilename = keithleyFilename;
            WayneKerrEmulationFilename = wayneKerrFilename;
        }

        public bool Emulate { get; private set; }

        public string KeithleyEmulationFilename { get; private set; }

        public string WayneKerrEmulationFilename { get; private set; }
    }
}
