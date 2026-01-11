using BeaconTelemetryHub.Receiver.Beacon.BeaconFinder;
using BeaconTelemetryHub.Receiver.Beacon.Models;
using BeaconTelemetryHub.Receiver.Bluetooth;
using BeaconTelemetryHub.Receiver.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace BeaconTelemetryHub.Receiver
{
    internal class BeaconBackgroundService(IBeaconFinder beaconFinder, 
                                           IBleAdapterBuilder bleAdapterBuilder,
                                           IOptions<BeaconReceiverSettings> settings)
                                           : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Starting beacon data receiver...");
            beaconFinder.BeaconFound += BeaconFinder_BeaconFound;
            string? adapterName = settings.Value.AdapterName;
            using IBleAdapter blAdapter = await bleAdapterBuilder.GetAdapter(adapterName);
            await beaconFinder.Search(blAdapter, stoppingToken);
        }

        private void BeaconFinder_BeaconFound(BeaconTelemetryBase telemetryData)
        {
            switch(telemetryData)
            {
                case BeaconTelemetryAExtended telemetryAExt:
                    Log.Debug("Beacon A Extended found: {Beacon}", telemetryAExt);
                    break;
                case BeaconTelemetryBExtended telemetryBExt:
                    Log.Debug("Beacon B Extended found: {Beacon}", telemetryBExt);
                    break;
                case BeaconTelemetryA telemetryA:
                    Log.Debug("Beacon A found: {Beacon}", telemetryA);
                    break;
                case BeaconTelemetryB telemetryB:
                    Log.Debug("Beacon B found: {Beacon}", telemetryB);
                    break;
                default:
                    Log.Warning("Unknown beacon type found: {Beacon}", telemetryData);
                    break;
            } 
        }
    }
}
