using BeaconTelemetryHub.Receiver.Beacon.Models;
using BeaconTelemetryHub.Receiver.Bluetooth;

namespace BeaconTelemetryHub.Receiver.Beacon.BeaconFinder
{
    /// <summary>
    /// Interface responsible for discovering Bluetooth Low Energy beacons.
    /// </summary>
    public interface IBeaconFinder
    {
        /// <summary>
        /// Starts the process of scanning for beacons using the provided BLE adapter.
        /// </summary>
        /// <param name="bleAdapter">The Bluetooth Low Energy adapter used for scanning.</param>
        /// <param name="cancellationToken">Token that allows cancelling the scan operation.</param>
        /// <returns>A task representing the asynchronous scan operation.</returns>
        public Task Search(IBleAdapter bleAdapter, CancellationToken cancellationToken);

        /// <summary>
        /// Event raised whenever a beacon is discovered.
        /// </summary>
        public event Action<BeaconTelemetryBase> BeaconFound;
    }

}
