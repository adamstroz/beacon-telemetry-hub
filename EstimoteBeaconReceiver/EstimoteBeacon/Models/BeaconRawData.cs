using EstimoteBeaconReceiver.Bluetooth;

namespace EstimoteBeaconReceiver.EstimoteBeacon.Models
{
    internal record BeaconRawData(string ServiceUUID, byte[] ServiceData, DateTimeOffset FoundTimeStamp, BleDeviceAddress BleDeviceAddress)
    {
        public override string ToString()
        {
            var serviceDataHex = ServiceData != null ? Convert.ToHexString(ServiceData) : "null";
            return $"ServiceUUID: {ServiceUUID}, ServiceData: {serviceDataHex}, FoundTimeStamp: {FoundTimeStamp:O}, BleDeviceAddress: {BleDeviceAddress}";
        }
    }
}
