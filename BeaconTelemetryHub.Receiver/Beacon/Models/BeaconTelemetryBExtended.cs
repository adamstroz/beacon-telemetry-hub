using UnitsNet;

namespace BeaconTelemetryHub.Receiver.Beacon.Models
{
    public readonly record struct MagneticFieldStrength(double X_axis,
                                                        double Y_axis,
                                                        double Z_axis);
    // This model applies to [Location Beacons] beacon type
    public record BeaconTelemetryBExtended(BeaconTelemetryB BaseTelemetry,
                                           MagneticFieldStrength MagneticFieldStrength,
                                           Illuminance AmbientLightLevel) : BeaconTelemetryB(BaseTelemetry);
}
