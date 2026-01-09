using EstimoteBeaconReceiver.Bluetooth;
using EstimoteBeaconReceiver.EstimoteBeacon.Models;
using EstimoteBeaconReceiver.EstimoteBeacon.PacketDataParser;
using EstimoteBeaconReceiver.Settings;
using Microsoft.Extensions.Options;
using Serilog;

namespace EstimoteBeaconReceiver.EstimoteBeacon.BeaconFinder
{

    internal class BeaconFinder(IBeaconTelemetryResolver beaconTelemetryResolver, 
                                IOptions<BeaconReceiverSettings> settings) : IBeaconFinder
    {
        public event Action<BeaconTelemetryA>? BeaconSubFrameARecived;
        public event Action<BeaconTelemetryB>? BeaconSubFrameBRecived;
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
                                    BeaconTelemetryA telemetryA = beaconTelemetryResolver.CreateTelemetry<BeaconTelemetryA>(rawData);
                                    BeaconSubFrameARecived?.Invoke(telemetryA);
                                    break;
                                }
                            case Type t when t == typeof(BeaconTelemetryB):
                                {
                                    BeaconTelemetryB telemetryB = beaconTelemetryResolver.CreateTelemetry<BeaconTelemetryB>(rawData);
                                    BeaconSubFrameBRecived?.Invoke(telemetryB);
                                    break;
                                }
                            case Type t when t == typeof(BeaconTelemetryAExtended):
                                {
                                    BeaconTelemetryAExtended telemetryAExtended = beaconTelemetryResolver.CreateTelemetry<BeaconTelemetryAExtended>(rawData);

                                    break;
                                }
                            case Type t when t == typeof(BeaconTelemetryBExtended):
                                {
                                    BeaconTelemetryBExtended telemetryBExtended = beaconTelemetryResolver.CreateTelemetry<BeaconTelemetryBExtended>(rawData);

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
                throw new InvalidOperationException("Failed to scan for Estimote beacons.", ex);
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
