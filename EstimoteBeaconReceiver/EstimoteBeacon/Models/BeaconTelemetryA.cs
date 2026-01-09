using EstimoteBeaconReceiver.Bluetooth;
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
    internal record BeaconTelemetryA(AccelerationData Acceleration,
                                     bool IsMoving,
                                     TimeSpan PreviousMotionStateDuration,
                                     TimeSpan CurrentMotionStateDuration,
                                     string BeaconIdentifier,
                                     short ProtocolVersion,
                                     DateTimeOffset FoundTimestamp,
                                     BleDeviceAddress BleDeviceAddress,
                                     GpioPins? GpioState = null,
                                     Pressure? Pressure = null,
                                     EstimoteErrorCodes? ErrorCodes = null) : BeaconTelemetryBase(BeaconIdentifier,
                                                                                                   ProtocolVersion,
                                                                                                   FoundTimestamp,
                                                                                                   BleDeviceAddress,
                                                                                                   ErrorCodes)
    {
        
    }
}
