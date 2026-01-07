using UnitsNet;

namespace EstimoteBeaconReceiver.EstimoteBeacon.Models
{
    internal readonly record struct GpioPins(bool Pin0,
                                             bool Pin1,
                                             bool Pin2,
                                             bool Pin3)
    {
    }
    internal readonly record struct AccelerationData(Acceleration X_axis,
                                                     Acceleration Y_axis,
                                                     Acceleration Z_axis)
    {
    }
    internal record EstimoteBeaconTelemetryA(string BeaconIdentifier, 
                                             short ProtocolVersion,
                                             AccelerationData Acceleration,
                                             bool IsMoving,
                                             TimeSpan PreviousMotionStateDuration,
                                             TimeSpan CurrentMotionStateDuration,
                                             GpioPins GpioState,
                                             Pressure? Pressure = null) : EstimoteBeaconTelemetryBase(BeaconIdentifier, ProtocolVersion)
    {
        
    }
}
