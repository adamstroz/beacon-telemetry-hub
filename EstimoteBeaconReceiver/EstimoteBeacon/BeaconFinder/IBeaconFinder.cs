using EstimoteBeaconReceiver.Bluetooth;
using EstimoteBeaconReceiver.EstimoteBeacon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimoteBeaconReceiver.EstimoteBeacon.BeaconFinder
{
    internal interface IBeaconFinder
    {
        public Task Search(IBleAdapter bleAdapter, CancellationToken cancellationToken);
        public event Action<BeaconTelemetryA> BeaconSubFrameARecived;
        public event Action<BeaconTelemetryB> BeaconSubFrameBRecived;
    }
}
