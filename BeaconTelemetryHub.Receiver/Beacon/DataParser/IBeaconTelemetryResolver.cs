using BeaconTelemetryHub.Receiver.Beacon.Models;

namespace BeaconTelemetryHub.Receiver.Beacon.DataParser
{
    internal interface IBeaconTelemetryResolver
    {
        public Type DetermineTelemeteryTypeFromRawData(BeaconRawData rawData);
        public T CreateTelemetry<T>(BeaconRawData rawData) where T : BeaconTelemetryBase;
    }
}
