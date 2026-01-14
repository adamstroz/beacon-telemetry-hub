using BeaconTelemetryHub.Receiver.Bluetooth.Models;

namespace BeaconTelemetryHub.Receiver.Beacon.Models
{
    public readonly record struct BeaconErrorCodes(bool HasFirmwareError,
                                                       bool HasClockError);
    public abstract record BeaconTelemetryBase(string BeaconIdentifier,
                                               short ProtocolVersion,
                                               DateTimeOffset FoundTimestamp,
                                               BleDeviceAddress BleDeviceAddress,
                                               BeaconErrorCodes? ErrorCodes = null)
    {
        public const byte SupportedProtocolMaximumVersion = 2;
    }
}
