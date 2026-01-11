using BeaconTelemetryHub.Receiver.Bluetooth.Models;

namespace BeaconTelemetryHub.Receiver.Beacon.Models
{
    internal readonly record struct BeaconErrorCodes(bool HasFirmwareError,
                                                       bool HasClockError);
    internal abstract record BeaconTelemetryBase(string BeaconIdentifier,
                                                 short ProtocolVersion,
                                                 DateTimeOffset FoundTimestamp,
                                                 BleDeviceAddress BleDeviceAddress,
                                                 BeaconErrorCodes? ErrorCodes = null)
    {
        public const byte SupportedProtocolMaximumVersion = 2;
    }
}
