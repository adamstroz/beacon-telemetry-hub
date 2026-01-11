using BeaconTelemetryHub.Receiver.Beacon.Models;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers
{
    internal abstract class TelemetryParserBase
    {
        private const byte ProtocolVersionMask = 0b1111_0000;
        private const int ExpectedServiceDataLengthForTelemetryType = 10;
        protected static string GetBeaconIdentifier(ReadOnlySpan<byte> rawServiceData)
        {
            if (rawServiceData.Length < ExpectedServiceDataLengthForTelemetryType)
            {
                throw new InvalidOperationException("Invalid service length, the service data is too short to get beacon identifier.");
            }
            return Convert.ToHexString(rawServiceData.Slice(1, 8));
        }
        protected static byte GetProtocolVersion(ReadOnlySpan<byte> rawServiceData)
        {
            if (rawServiceData.Length < ExpectedServiceDataLengthForTelemetryType)
            {
                throw new InvalidOperationException("Invalid service length, the service data is too short to get protocol version.");
            }
            return (byte)((rawServiceData[0] & ProtocolVersionMask) >> 4);
        }
        protected static void ThrowIfProtocolVersionInvalid(byte protocolVersion)
        {
            if (protocolVersion > BeaconTelemetryBase.SupportedProtocolMaximumVersion)
            {
                throw new NotSupportedException($"Protocol version '{protocolVersion}' is not supported." +
                                                $" Maximum supported protocol version is '{BeaconTelemetryBase.SupportedProtocolMaximumVersion}'");
            }
        }

    }
}
