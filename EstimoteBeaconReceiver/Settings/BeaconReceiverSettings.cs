using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimoteBeaconReceiver.Settings
{
    internal record BeaconReceiverSettings()
    {
        public string? AdapterName { get; init; } = null;
    }
}
