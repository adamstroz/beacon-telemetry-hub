using UnitsNet;

namespace BeaconTelemetryHub.Receiver.Beacon.Models
{
    // This model applies to [Location Beacons] beacon type
    public record BeaconTelemetryBExtended(BeaconTelemetryB BaseTelemetry,
                                           MagneticFieldStrength MagneticFieldStrength,
                                           Illuminance AmbientLightLevel) : BeaconTelemetryB(BaseTelemetry)
    {
    }
}
