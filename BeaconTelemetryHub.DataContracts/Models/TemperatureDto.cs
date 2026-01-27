namespace BeaconTelemetryHub.DataContracts.Models
{
    public record TemperatureDto(double TemperatureCelsius, 
                                 string DeviceIdentifier,
                                 string DeviceBleAddress,
                                 DateTimeOffset FoundTimestamp) : DtoBase(DeviceIdentifier, DeviceBleAddress, FoundTimestamp);

}
