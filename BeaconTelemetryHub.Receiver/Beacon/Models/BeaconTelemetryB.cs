using BeaconTelemetryHub.Receiver.Bluetooth.Models;
using UnitsNet;

namespace BeaconTelemetryHub.Receiver.Beacon.Models
{
    internal readonly record struct MagneticFieldStrength(double X_axis,
                                                          double Y_axis,
                                                          double Z_axis);
    internal record BeaconTelemetryB(TimeSpan UpTime,
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
