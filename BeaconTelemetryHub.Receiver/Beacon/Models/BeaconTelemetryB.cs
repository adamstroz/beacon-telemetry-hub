using BeaconTelemetryHub.Receiver.Bluetooth.Models;
using UnitsNet;

namespace BeaconTelemetryHub.Receiver.Beacon.Models
{
    public record BeaconTelemetryB(TimeSpan UpTime,
                                   Temperature Temperature,
                                   string BeaconIdentifier,
                                   short ProtocolVersion,
                                   DateTimeOffset FoundTimestamp,
                                   BleDeviceAddress BleDeviceAddress,
                                   short Rssi,
                                   Ratio? BatteryLevel = null,
                                   ElectricPotential? BatteryVoltage = null,
                                   BeaconErrorCodes? ErrorCodes = null) : BeaconTelemetryBase(BeaconIdentifier,
                                                                                              ProtocolVersion,
                                                                                              FoundTimestamp, 
                                                                                              BleDeviceAddress,
                                                                                              Rssi,
                                                                                              ErrorCodes);
}
