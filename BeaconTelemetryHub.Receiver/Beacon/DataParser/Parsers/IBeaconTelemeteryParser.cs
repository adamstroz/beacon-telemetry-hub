using BeaconTelemetryHub.Receiver.Beacon.Models;

namespace BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers
{
    public interface IBeaconTelemeteryGeneralParser
    {
        Type SupportedType { get; }
        BeaconTelemetryBase Parse(BeaconRawData data);
    }
    public interface IBeaconTelemeteryDetailedParser<out T> : IBeaconTelemeteryGeneralParser where T : BeaconTelemetryBase
    {
        new T Parse(BeaconRawData rawData);
    }
}
