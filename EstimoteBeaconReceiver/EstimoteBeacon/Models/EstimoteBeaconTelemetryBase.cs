namespace EstimoteBeaconReceiver.EstimoteBeacon.Models
{
    internal readonly record struct EstimoteErrorCodes(bool HasFirmwareError,
                                                       bool HasClockError)
    {
    }
    internal abstract record EstimoteBeaconTelemetryBase(string BeaconIdentifier,
                                                         short ProtocolVersion,
                                                         EstimoteErrorCodes? Errors = null)
    {

    }
}
