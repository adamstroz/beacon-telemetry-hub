using BeaconTelemetryHub.Receiver.Bluetooth.Models;

namespace BeaconTelemetryHub.Receiver.Beacon.Models
{
    public record BeaconRawData(string ServiceUUID, byte[] ServiceData, DateTimeOffset FoundTimeStamp, BleDeviceAddress BleDeviceAddress)
    {
        public override string ToString()
        {
            return $"ServiceUUID: {ServiceUUID}, ServiceData: {BitConverter.ToString(ServiceData)}, FoundTimeStamp: {FoundTimeStamp}, BleDeviceAddress: {BleDeviceAddress}";
        }
    }
}
