using EstimoteBeaconReceiver.Bluetooth;
using EstimoteBeaconReceiver.EstimoteBeacon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimoteBeaconReceiver.EstimoteBeacon
{
    internal interface IBeaconFinder
    {
        public Task StartSearching(IBleAdapter bleAdapter, TimeSpan searchTime, CancellationToken cancellationToken);
        public event Action<EstimoteBeaconTelemetryBase> BeaconDataRecived;
    }
}
