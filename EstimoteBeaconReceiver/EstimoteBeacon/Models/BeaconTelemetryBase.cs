using EstimoteBeaconReceiver.Bluetooth;

namespace EstimoteBeaconReceiver.EstimoteBeacon.Models
{
    internal readonly record struct EstimoteErrorCodes(bool HasFirmwareError,
                                                       bool HasClockError)
    {
    }
    internal abstract record BeaconTelemetryBase(string BeaconIdentifier,
                                                short ProtocolVersion,
                                                DateTimeOffset FoundTimestamp,
                                                BleDeviceAddress BleDeviceAddress,
                                                EstimoteErrorCodes? ErrorCodes = null)
    {
        public const byte SupportedProtocolMinimumVersion = 2;
    }
}
