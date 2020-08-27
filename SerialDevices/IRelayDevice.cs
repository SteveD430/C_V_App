using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_V_App.SerialDevices
{
    public interface IRelayDevice : ISerialDevice
    {
        int AvailableRelayStations { get; }

        IEnumerable<ConfiguredRelayStation> ConfiguredRelayStations { get; }

        void SwitchOnRelay(int relay);

        void DisableRelays();

        void Reset();
    }
}
