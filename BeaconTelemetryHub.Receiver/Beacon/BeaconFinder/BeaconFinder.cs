using BeaconTelemetryHub.Receiver.Beacon;
using BeaconTelemetryHub.Receiver.Beacon.DataParser;
using BeaconTelemetryHub.Receiver.Beacon.Models;
using BeaconTelemetryHub.Receiver.Bluetooth;
using BeaconTelemetryHub.Receiver.Bluetooth.Models;
using BeaconTelemetryHub.Receiver.Settings;
using Microsoft.Extensions.Options;
using Serilog;

namespace BeaconTelemetryHub.Receiver.Beacon.BeaconFinder
{

    internal class BeaconFinder(IBeaconTelemetryResolver beaconTelemetryResolver, 
                                IOptions<BeaconReceiverSettings> settings) : IBeaconFinder
    {
        public event Action<BeaconTelemetryBase>? BeaconFound;

        private void ParseFoundPackets(IReadOnlyDictionary<BleDeviceAddress, BleDeviceAdvertisementPacket> packets)
        {
            try
            {
                foreach (KeyValuePair<BleDeviceAddress, BleDeviceAdvertisementPacket> packet in packets)
                {
                    if (packet.Value.IsEstimoteTelemetryPacket(settings.Value.EstimoteServiceUUID, settings.Value.EstimoteTelemetryPacketTypeId))
                    {
                        BeaconRawData rawData = packet.Value.GetBeaconRawData(settings.Value.EstimoteServiceUUID);
                        switch (beaconTelemetryResolver.DetermineTelemeteryTypeFromRawData(rawData))
                        {
                            case Type t when t == typeof(BeaconTelemetryA):
                                {
                                    BeaconFound?.Invoke(beaconTelemetryResolver.CreateTelemetry<BeaconTelemetryA>(rawData));
                                    break;
                                }
                            case Type t when t == typeof(BeaconTelemetryB):
                                {
                                    BeaconFound?.Invoke(beaconTelemetryResolver.CreateTelemetry<BeaconTelemetryB>(rawData));
                                    break;
                                }
                            case Type t when t == typeof(BeaconTelemetryAExtended):
                                {
                                    BeaconFound?.Invoke(beaconTelemetryResolver.CreateTelemetry<BeaconTelemetryAExtended>(rawData));
                                    break;
                                }
                            case Type t when t == typeof(BeaconTelemetryBExtended):
                                {
                                    BeaconFound?.Invoke(beaconTelemetryResolver.CreateTelemetry<BeaconTelemetryBExtended>(rawData));
                                    break;
                                }
                            default:
                                throw new NotSupportedException("The determined telemetry type is not supported.");
                        }
                    }
                }
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to parse found Ble advertisment packets.", ex);
            }
        }
        private async Task ScanOnce(IBleAdapter bleAdapter, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(bleAdapter);
            IReadOnlyDictionary<BleDeviceAddress, BleDeviceAdvertisementPacket> packets;
            try
            {
                packets = await bleAdapter.DiscoveryAdvertisementPackets(settings.Value.ScanDuration, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to scan for advertisement packets.", ex);
            }
            if (packets.Count == 0)
            {
                return;
            }
            ParseFoundPackets(packets);
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
