using BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers;
using BeaconTelemetryHub.Receiver.Beacon.Models;
using Serilog;
using System.Buffers.Binary;

namespace BeaconTelemetryHub.Receiver.Beacon.DataParser
{
    public class BeaconTelemetryResolver : IBeaconTelemetryResolver
    {
        public const byte TelemetryTypeMask = 0b0000_0011;
        public const byte TelemetryTypeA = 0x00;
        public const byte TelemetryTypeB = 0x01;
        private const int ExpectedServiceDataLengthForTelemetryType = 10;
        private readonly IEnumerable<IBeaconTelemeteryGeneralParser> _parsers;
        public BeaconTelemetryResolver(IEnumerable<IBeaconTelemeteryGeneralParser> parsers)
        {
            _parsers = parsers;
            Log.Verbose("Registered parsers: {Type}", string.Join(";", _parsers.Select(x => x.SupportedType)));
        }
        public Type DetermineTelemeteryTypeFromRawData(BeaconRawData rawData)  
        {
            ArgumentNullException.ThrowIfNull(rawData, nameof(rawData));
            ReadOnlySpan<byte> data = rawData.ServiceData;
            if (data.Length < ExpectedServiceDataLengthForTelemetryType)
            {
                throw new InvalidOperationException($"Invalid service length, the service data is too short (length '{rawData.ServiceData.Length}') " +
                                                    $"to determine telemetry type. Expected length: {ExpectedServiceDataLengthForTelemetryType}");
            }
            try
            {
                byte type = (byte)(data[9] & TelemetryTypeMask);
                switch (type)
                {
                    case TelemetryTypeA:
                        //Check if telemetry contains valid extended data (GPIO state at byte 15 and athmospheric pressure at bytes 16,17,18,19)
                        if (BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(16, 4)) != 0xFFFFFFFF &&
                            data[15] != 0xF8)
                        {
                            return typeof(BeaconTelemetryAExtended);
                        }
                        return typeof(BeaconTelemetryA);
                    case TelemetryTypeB:
                        //Check if telemetry contains valid extended data (Ambient light level at byte 13, magnetometer readings at bytes 10,11,12 can be any 8-bit numbers)
                        if (data[13] != 0xFF)
                        {
                            return typeof(BeaconTelemetryBExtended);
                        }
                        return typeof(BeaconTelemetryB);
                    default:
                        throw new NotSupportedException($"Telemetry type '{type}' is not supported.");
                }
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Failed to determine telemetry type from raw data. Data: {Convert.ToHexString(data)}", ex);
            }
        }
        public T CreateTelemetry<T>(BeaconRawData rawData) where T : BeaconTelemetryBase
        {
            ArgumentNullException.ThrowIfNull(rawData, nameof(rawData));
            Type telemetryType;
            try
            {
                telemetryType = DetermineTelemeteryTypeFromRawData(rawData);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Failed to create telemetery of type {typeof(T)}, failed to determine type from raw data", ex);
            }
            if(telemetryType != typeof(T))
            {
                throw new InvalidOperationException($"The provided raw data is not of the expected telemetry type '{typeof(T)}'. Detected type: '{telemetryType}'");
            }
            IBeaconTelemeteryDetailedParser<T> selectedParser; 

            try
            {
                selectedParser = _parsers.OfType<IBeaconTelemeteryDetailedParser<T>>().First();
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"No parser found for telemetry type '{typeof(T)}'.", ex);
            }

            try
            {
                return selectedParser.Parse(rawData);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse raw data to create telemetry of type '{typeof(T)}'.", ex);
            }
        }
    }
}

