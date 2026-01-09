using EstimoteBeaconReceiver.EstimoteBeacon.Models;
using EstimoteBeaconReceiver.EstimoteBeacon.PacketDataParser;
using Serilog;
using UnitsNet;

namespace EstimoteBeaconReceiver.EstimoteBeacon.DataParser
{
    internal class BeaconTelemetryResolver : IBeaconTelemetryResolver
    {
        public const byte TelemetryTypeMask = 0b0000_0011;
        public const byte TelemetryTypeA = 0x00;
        public const byte TelemetryTypeB = 0x01;
        private const int ExpectedServiceDataLengthForTelemetryType = 10;
        IEnumerable<IBeaconTelemeteryGeneralParser> _parsers;
        public BeaconTelemetryResolver(IEnumerable<IBeaconTelemeteryGeneralParser> parsers)
        {
            _parsers = parsers;
            Log.Debug("Registered parsers: {Type}", string.Join(";", _parsers.Select(x => x.SupportedType)));
        }
        public Type DetermineTelemeteryTypeFromRawData(BeaconRawData rawData)  
        {
            ArgumentNullException.ThrowIfNull(rawData, nameof(rawData)); 
            if (rawData.ServiceData.Length < ExpectedServiceDataLengthForTelemetryType)
            {
                throw new InvalidOperationException($"Invalid service length, the Estimote service data is too short (length '{rawData.ServiceData.Length}') " +
                                                    $"to determine telemetry type. Expected length: {ExpectedServiceDataLengthForTelemetryType}");
            }
            byte type = (byte)(rawData.ServiceData[9] & TelemetryTypeMask);
            return type switch
            {
                TelemetryTypeA => typeof(BeaconTelemetryA),
                TelemetryTypeB => typeof(BeaconTelemetryB),
                _ => throw new NotSupportedException($"Telemetry type '{type}' is not supported."),
            };
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

