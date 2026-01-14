using BeaconTelemetryHub.Receiver.Bluetooth.Models;
using UnitsNet;

namespace BeaconTelemetryHub.Receiver.Beacon.Models
{
    public readonly record struct GpioPins(bool Pin0,
                                             bool Pin1,
                                             bool Pin2,
                                             bool Pin3);
    public readonly record struct AccelerationData(Acceleration X_axis,
                                                     Acceleration Y_axis,
                                                     Acceleration Z_axis);
    public record BeaconTelemetryA(AccelerationData Acceleration,
                                   bool IsMoving,
                                   TimeSpan PreviousMotionStateDuration,
                                   TimeSpan CurrentMotionStateDuration,
                                   string BeaconIdentifier,
                                   short ProtocolVersion,
                                   DateTimeOffset FoundTimestamp,
                                   BleDeviceAddress BleDeviceAddress,
                                   BeaconErrorCodes? ErrorCodes = null) : BeaconTelemetryBase(BeaconIdentifier,
                                                                                              ProtocolVersion,
                                                                                              FoundTimestamp,
                                                                                              BleDeviceAddress,
                                                                                              ErrorCodes);
}
