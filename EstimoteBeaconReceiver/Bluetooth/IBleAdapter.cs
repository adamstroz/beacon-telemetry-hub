using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimoteBeaconReceiver.Bluetooth
{
    internal interface IBleAdapter
    {
        public Task StartDiscoveryAsync();
        public Task StopDiscoveryAsync();

    }
}
