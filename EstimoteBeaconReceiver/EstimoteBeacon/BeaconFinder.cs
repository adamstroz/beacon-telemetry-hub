using EstimoteBeaconReceiver.Bluetooth;
using EstimoteBeaconReceiver.EstimoteBeacon.Models;
using EstimoteBeaconReceiver.Settings;
using Microsoft.Extensions.Options;
using Serilog;

namespace EstimoteBeaconReceiver.EstimoteBeacon
{

    internal class BeaconFinder(IOptions<BeaconReceiverSettings> settings) : IBeaconFinder
    {
        public event Action<EstimoteBeaconTelemetryA>? BeaconSubFrameARecived;
        public event Action<EstimoteBeaconTelemetryB>? BeaconSubFrameBRecived;
        private async Task ScanOnce(IBleAdapter bleAdapter, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(bleAdapter);
            try
            {
                IReadOnlyDictionary<BleDeviceAddress, BleDeviceAdvertisementPacket> packets = await bleAdapter.DiscoveryAdvertisementPackets(settings.Value.ScanDuration, cancellationToken);
                foreach (KeyValuePair<BleDeviceAddress, BleDeviceAdvertisementPacket> packet in packets)
                {
                    if (packet.Value.IsEstimoteTelemetryPacket(settings.Value.EstimoteServiceUUID, settings.Value.EstimoteTelemetryPacketTypeId))
                    {
                        Log.Debug($"Estimote beacon detected!");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error during scanning BLE advertisement packets.", ex);

            }
        }
        public async Task Search(IBleAdapter bleAdapter, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(bleAdapter);
            while (cancellationToken.IsCancellationRequested == false)
            {
                await ScanOnce(bleAdapter, cancellationToken);
                await Task.Delay(settings.Value.ScanInterval, cancellationToken);
            }
        } 
    }
}
