using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimoteBeaconReceiver.Bluetooth
{
    internal readonly struct BleDeviceAddress : IEquatable<BleDeviceAddress>
    {
        private readonly string _address;
        public string Address => _address;
        public BleDeviceAddress(string address)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(address, nameof(address));
            ThrowIfAddressInvalid(address);
            _address = address;
        }
        public BleDeviceAddress()
        {
            throw new InvalidOperationException("BLE device address must be provided.");
        }
        private static void ThrowIfAddressInvalid(string address)
        {
            // BLE address format: "XX:XX:XX:XX:XX:XX" where X is a hex digit
            if (address.Length != 17)
                throw new ArgumentException($"Invalid BLE address format: {address}", nameof(address));

            for (int i = 0; i < address.Length; i++)
            {
                if ((i + 1) % 3 == 0)
                {
                    if (address[i] != ':')
                        throw new ArgumentException($"Invalid BLE address format: {address}", nameof(address));
                }
                else
                {
                    if (!Uri.IsHexDigit(address[i]))
                        throw new ArgumentException($"Invalid BLE address format: {address}", nameof(address));
                }
            }
        }
        public override string ToString() => Address;
        public bool Equals(BleDeviceAddress other) => string.Equals(_address, other._address, StringComparison.OrdinalIgnoreCase);
        public override bool Equals(object? obj) => obj is BleDeviceAddress other && Equals(other);
        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(_address);
        public static bool operator ==(BleDeviceAddress left, BleDeviceAddress right) => left.Equals(right);
        public static bool operator !=(BleDeviceAddress left, BleDeviceAddress right) => !left.Equals(right);
    }

    internal record BleDeviceAdvertisementPacket(BleDeviceAddress Address, short Rssi, FrozenDictionary<string, object>? ServiceData)
    {
        public DateTimeOffset FoundTimestamp { get; } = DateTimeOffset.UtcNow;
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"Address: {Address}, RSSI: {Rssi}dBm");
            if (ServiceData != null)
            {
                foreach (var entry in ServiceData)
                {
                    sb.AppendLine($"  ServiceData Key: {entry.Key}, Value: {BitConverter.ToString((byte[])entry.Value)}");
                }
            }   
            
            return sb.ToString();
        }
        public bool IsEstimoteTelemetryPacket(string estimoteServiceUUID, byte estimoteTelemetryPacketTypeId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(estimoteServiceUUID, nameof(estimoteServiceUUID));
            if (ServiceData == null)
            {
                return false;
            }
            if(ServiceData.Keys.Any(k => k.Contains(estimoteServiceUUID, StringComparison.OrdinalIgnoreCase)) == false)
            {
                return false;
            }
            // At this point we know that the packet contains Service Data with the Estimote UUID (packet is from an Estimote device)
            // Check for the telemetry packet type identifier
            if (ServiceData.Values.Any(s => {
                if (s is byte[] data)
                {
                    if (data.Length < 1)
                    {
                        return false;
                    }
                    if ((data[0] & 0b0000_1111) == estimoteTelemetryPacketTypeId)
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
    