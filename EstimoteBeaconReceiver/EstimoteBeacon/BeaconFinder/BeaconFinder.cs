using EstimoteBeaconReceiver.Bluetooth;
using EstimoteBeaconReceiver.EstimoteBeacon.Models;
using EstimoteBeaconReceiver.EstimoteBeacon.PacketDataParser;
using EstimoteBeaconReceiver.Settings;
using Microsoft.Extensions.Options;
using Serilog;

namespace EstimoteBeaconReceiver.EstimoteBeacon.BeaconFinder
{

    internal class BeaconFinder(IBeaconTelemetryResolver estimoteBeaconDataParser, 
                                IOptions<BeaconReceiverSettings> settings) : IBeaconFinder
    {
        public event Action<BeaconTelemetryA>? BeaconSubFrameARecived;
        public event Action<BeaconTelemetryB>? BeaconSubFrameBRecived;
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
                        BeaconRawData rawData = packet.Value.GetBeaconRawData(settings.Value.EstimoteServiceUUID);
                        switch (estimoteBeaconDataParser.DetermineTelemeteryTypeFromRawData(rawData))
                        {
                            case Type t when t == typeof(BeaconTelemetryA):
                            {
                                BeaconTelemetryA telemetryA = estimoteBeaconDataParser.CreateTelemetry<BeaconTelemetryA>(rawData);
                                Log.Debug($"Beacon SubFrame A received: {telemetryA}");
                                BeaconSubFrameARecived?.Invoke(telemetryA);
                                break;
                            }
                            case Type t when t == typeof(BeaconTelemetryB):
                            {
                                BeaconTelemetryB telemetryB = estimoteBeaconDataParser.CreateTelemetry<BeaconTelemetryB>(rawData);
                                Log.Debug($"Beacon SubFrame B received: {telemetryB}");
                                BeaconSubFrameBRecived?.Invoke(telemetryB);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               Log.Error("Error during scanning BLE advertisement packets.", ex);

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
