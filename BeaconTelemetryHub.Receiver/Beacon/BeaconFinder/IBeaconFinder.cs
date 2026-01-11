using BeaconTelemetryHub.Receiver.Beacon.Models;
using BeaconTelemetryHub.Receiver.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaconTelemetryHub.Receiver.Beacon.BeaconFinder
{
    internal interface IBeaconFinder
    {
        public Task Search(IBleAdapter bleAdapter, CancellationToken cancellationToken);
        public event Action<BeaconTelemetryBase> BeaconFound;
    }
}
