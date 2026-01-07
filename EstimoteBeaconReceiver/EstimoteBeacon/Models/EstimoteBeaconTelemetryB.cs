using UnitsNet;

namespace EstimoteBeaconReceiver.EstimoteBeacon.Models
{
    internal readonly record struct MagneticFieldStrength(MagneticField X_axis,
                                                          MagneticField Y_axis,
                                                          MagneticField Z_axis)
    {
    }
    internal record EstimoteBeaconTelemetryB(string BeaconIdentifier,
                                             short ProtocolVersion,
                                             MagneticFieldStrength MagneticFieldStrength,
                                             double AmbientLightLevel,
                                             TimeSpan UpTime,
                                             Temperature Temperature,
                                             Ratio BatteryLevel): EstimoteBeaconTelemetryBase(BeaconIdentifier, ProtocolVersion)
    {
    }
}
