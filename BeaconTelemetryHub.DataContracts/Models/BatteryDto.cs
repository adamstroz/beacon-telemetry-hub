namespace BeaconTelemetryHub.DataContracts.Models
{
    public record BatteryDto(double BatteryLevelPercent,
                             double BatteryVoltageMillivolts,
                             string DeviceIdentifier,
                             string DeviceBleAddress,
                             DateTimeOffset FoundTimestamp) : DtoBase(DeviceIdentifier, DeviceBleAddress, FoundTimestamp);
}
