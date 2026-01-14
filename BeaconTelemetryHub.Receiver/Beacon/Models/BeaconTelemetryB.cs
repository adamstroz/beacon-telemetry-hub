using BeaconTelemetryHub.Receiver.Bluetooth.Models;
using UnitsNet;

namespace BeaconTelemetryHub.Receiver.Beacon.Models
{
    public readonly record struct MagneticFieldStrength(double X_axis,
                                                        double Y_axis,
                                                        double Z_axis);
    public record BeaconTelemetryB(TimeSpan UpTime,
                                   Temperature Temperature,
                                   string BeaconIdentifier,
                                   short ProtocolVersion,
                                   DateTimeOffset FoundTimestamp,
                                   BleDeviceAddress BleDeviceAddress,
                                   Ratio? BatteryLevel = null,
                                   ElectricPotential? BatteryVoltage = null,
                                   BeaconErrorCodes? ErrorCodes = null) : BeaconTelemetryBase(BeaconIdentifier,
                                                                                              ProtocolVersion,
                                                                                              FoundTimestamp, 
                                                                                              BleDeviceAddress,
                                                                                              ErrorCodes);
}
