using UnitsNet;

namespace BeaconTelemetryHub.Receiver.Beacon.Models
{
    // This model applies to [Location Beacons] beacon type
    public record BeaconTelemetryAExtended(BeaconTelemetryA BaseTelemetry,
                                           GpioPins GpioState,
                                           Pressure Pressure) : BeaconTelemetryA(BaseTelemetry)
    {
    }
}
