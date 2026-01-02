using EstimoteBeaconReceiver.Bluetooth;
using EstimoteBeaconReceiver.EstimoteBeacon;
using EstimoteBeaconReceiver.EstimoteBeacon.Models;
using EstimoteBeaconReceiver.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace EstimoteBeaconReceiver
{
    internal class BeaconBackgroundService(IBeaconFinder beaconFinder, 
                                           IBleAdapterFinder bleAdapterFinder,
                                           IOptions<BeaconReceiverSettings> settings)
                                           : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Starting Estimote beacon data receiver...");
            string? adapterName = settings.Value.AdapterName;
            IBleAdapter blAdapter = await bleAdapterFinder.GetAdapter(adapterName);
            beaconFinder.BeaconDataRecived += BeaconFinder_BeaconDataRecived;
            await beaconFinder.StartSearching(blAdapter, TimeSpan.Zero, stoppingToken);
        }

        private void BeaconFinder_BeaconDataRecived(EstimoteBeaconTelemetryBase obj)
        {
            throw new NotImplementedException();
        }
    }
}
