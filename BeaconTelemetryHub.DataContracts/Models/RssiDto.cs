namespace BeaconTelemetryHub.DataContracts.Models
{
    public record RssiDto(short Rssi,
                          string DeviceIdentifier,
                          string DeviceBleAddress,
                          DateTimeOffset FoundTimestamp) : DtoBase(DeviceIdentifier, DeviceBleAddress, FoundTimestamp);
}
