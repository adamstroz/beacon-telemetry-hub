using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimoteBeaconReceiver.Bluetooth
{
    internal interface IBleAdapter : IDisposable
    {
        public string Name { get; }
        public Task<IReadOnlyDictionary<BleDeviceAddress, BleDeviceAdvertisementPacket>> DiscoveryAdvertisementPackets(TimeSpan scanDuration, CancellationToken cancellationToken);

    }
}
