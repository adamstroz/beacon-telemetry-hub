using EstimoteBeaconReceiver.Bluetooth;
using EstimoteBeaconReceiver.EstimoteBeacon.Models;
using Linux.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimoteBeaconReceiver.EstimoteBeacon
{
    internal class BeaconFinder : IBeaconFinder
    {
        public event Action<EstimoteBeaconTelemetryBase> BeaconDataRecived;

        public async Task StartSearching(IBleAdapter bleAdapter, TimeSpan searchTime, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(bleAdapter);
            cancellationToken.ThrowIfCancellationRequested();





            try
            {
                await bleAdapter.StartDiscoveryAsync();
            }
            catch (Exception ex)
            {

            }

 
        }
    }
}
