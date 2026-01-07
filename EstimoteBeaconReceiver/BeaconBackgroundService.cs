using EstimoteBeaconReceiver.Bluetooth;
using EstimoteBeaconReceiver.EstimoteBeacon;
using EstimoteBeaconReceiver.EstimoteBeacon.Models;
using EstimoteBeaconReceiver.Settings;
 
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Tmds.DBus;

namespace EstimoteBeaconReceiver
{
    internal class BeaconBackgroundService(IBeaconFinder beaconFinder, 
                                           IBleAdapterBuilder bleAdapterBuilder,
                                           IOptions<BeaconReceiverSettings> settings)
                                           : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Starting Estimote beacon data receiver...");
            string? adapterName = settings.Value.AdapterName;
            using IBleAdapter blAdapter = await bleAdapterBuilder.GetAdapter(adapterName);
            beaconFinder.BeaconSubFrameARecived += BeaconFinder_BeaconSubFrameARecived;
            beaconFinder.BeaconSubFrameBRecived += BeaconFinder_BeaconSubFrameBRecived;
            await beaconFinder.Search(blAdapter, stoppingToken);
        }

        private void BeaconFinder_BeaconSubFrameBRecived(EstimoteBeaconTelemetryB obj)
        {
           
        }

        private void BeaconFinder_BeaconSubFrameARecived(EstimoteBeaconTelemetryA obj)
        {
           
        }
    }
}
