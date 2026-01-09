using EstimoteBeaconReceiver.Bluetooth;
using UnitsNet;

namespace EstimoteBeaconReceiver.EstimoteBeacon.Models
{
    internal readonly record struct MagneticFieldStrength(double X_axis,
                                                          double Y_axis,
                                                          double Z_axis)
    {
    }
    internal sealed record BeaconTelemetryB(TimeSpan UpTime,
                                            Temperature Temperature,
                                            string BeaconIdentifier,
                                            short ProtocolVersion,
                                            DateTimeOffset FoundTimestamp,
                                            BleDeviceAddress BleDeviceAddress,
                                            MagneticFieldStrength? MagneticFieldStrength = null,
                                            Ratio? BatteryLevel = null,
                                            ElectricPotential? BatteryVoltage = null,
                                            Illuminance? AmbientLightLevel = null,
                                            EstimoteErrorCodes? ErrorCodes = null) 
                                            : BeaconTelemetryBase(BeaconIdentifier,
                                                                            ProtocolVersion,
                                                                            FoundTimestamp, 
                                                                            BleDeviceAddress,
                                                                            ErrorCodes)
    {
    }
}
