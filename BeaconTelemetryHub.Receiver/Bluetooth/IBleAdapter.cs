using BeaconTelemetryHub.Receiver.Bluetooth.Models;

namespace BeaconTelemetryHub.Receiver.Bluetooth
{
    /// <summary>
    /// Represents a Bluetooth Low Energy (BLE) adapter capable of devices discovery and advertisement packets retrieval.
    /// </summary>
    public interface IBleAdapter : IDisposable
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
