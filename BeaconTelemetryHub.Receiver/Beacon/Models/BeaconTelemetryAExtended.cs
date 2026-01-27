using UnitsNet;

namespace BeaconTelemetryHub.Receiver.Beacon.Models
{
    public readonly record struct GpioPins(bool Pin0,
                                           bool Pin1,
                                           bool Pin2,
                                           bool Pin3);
    // This model applies to [Location Beacons] beacon type
    public record BeaconTelemetryAExtended(BeaconTelemetryA BaseTelemetry,
                                           GpioPins GpioState,
                                           Pressure Pressure) : BeaconTelemetryA(BaseTelemetry);
}
