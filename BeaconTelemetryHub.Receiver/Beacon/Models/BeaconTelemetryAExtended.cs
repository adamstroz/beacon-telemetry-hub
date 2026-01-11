using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace BeaconTelemetryHub.Receiver.Beacon.Models
{
    // This model applies to [Location Beacons] beacon type
    internal record BeaconTelemetryAExtended(BeaconTelemetryA BaseTelemetry,
                                             GpioPins GpioState,
                                             Pressure Pressure) : BeaconTelemetryA(BaseTelemetry)
    {
    }
}
