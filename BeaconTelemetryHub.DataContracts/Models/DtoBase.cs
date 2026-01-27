namespace BeaconTelemetryHub.DataContracts.Models
{
    public abstract record DtoBase(string DeviceIdentifier,
                                   string DeviceBleAddress,
                                   DateTimeOffset FoundTimestamp);
}
