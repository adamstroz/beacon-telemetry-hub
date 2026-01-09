using EstimoteBeaconReceiver.Bluetooth;
using EstimoteBeaconReceiver.EstimoteBeacon.Models;

namespace EstimoteBeaconReceiver.EstimoteBeacon
{
    internal static class BleAdvertisementPacketExtensions
    {
        public const byte EstimotePacketTypeMask = 0b0000_1111;
        internal static BeaconRawData GetBeaconRawData(this BleDeviceAdvertisementPacket packet, string estimoteServiceUUID)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(estimoteServiceUUID);
            ArgumentNullException.ThrowIfNull(packet, nameof(packet));
            ArgumentNullException.ThrowIfNull(packet.ServiceData, nameof(packet.ServiceData));
            KeyValuePair<string, object> estimoteServiceData;
            try
            {
                estimoteServiceData = packet.ServiceData.First(kv => kv.Key.Contains(estimoteServiceUUID, StringComparison.OrdinalIgnoreCase));
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("The provided packet does not contain Estimote service data.", ex);
            }
            if (estimoteServiceData.Value is not byte[] data)
            {
                throw new InvalidOperationException("The Estimote service data is not in the expected format (byte array).");
            }
            return new BeaconRawData(estimoteServiceData.Key, (byte[])data.Clone(), packet.FoundTimestamp, packet.Address);
        }
        internal static bool IsEstimoteTelemetryPacket(this BleDeviceAdvertisementPacket packet, string estimoteServiceUUID, byte estimoteTelemetryPacketTypeId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(estimoteServiceUUID, nameof(estimoteServiceUUID));
            if (packet.ServiceData == null)
            {
                return false;
            }
            if (packet.ServiceData.Keys.Any(k => k.Contains(estimoteServiceUUID, StringComparison.OrdinalIgnoreCase)) == false)
            {
                return false;
            }
            // At this point we know that the packet contains Service Data with the Estimote UUID (packet is from an Estimote device)
            // Check for the telemetry packet type identifier
            if (packet.ServiceData.Values.Any(s => {
                if (s is byte[] data)
                {
                    if (data.Length < 1)
                    {
                        return false;
                    }
                    if ((data[0] & EstimotePacketTypeMask) == estimoteTelemetryPacketTypeId)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
                return false;
            }))
            {
                return true;
            }
            return false;
        }
    }
}
