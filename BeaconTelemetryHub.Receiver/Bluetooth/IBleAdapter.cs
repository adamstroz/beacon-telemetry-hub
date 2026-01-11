using BeaconTelemetryHub.Receiver.Bluetooth.Models;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaconTelemetryHub.Receiver.Bluetooth
{
    /// <summary>
    /// Represents a Bluetooth Low Energy (BLE) adapter capable of device discovery and advertisement packet retrieval.
    /// </summary>
    internal interface IBleAdapter : IDisposable
    {
        /// <summary>
        /// Gets the name of the BLE adapter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Discovers BLE devices and retrieves their advertisement packets within the specified scan duration.
        /// </summary>
        /// <param name="scanDuration">The duration to scan for BLE devices.</param>
        /// <param name="cancellationToken">A token to cancel the discovery operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a read-only dictionary
        /// mapping device addresses to their advertisement packets.
        /// </returns>
        public Task<IReadOnlyDictionary<BleDeviceAddress, BleDeviceAdvertisementPacket>> DiscoveryAdvertisementPackets(TimeSpan scanDuration, CancellationToken cancellationToken);
    }
}
