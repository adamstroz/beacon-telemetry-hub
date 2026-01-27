using BeaconTelemetryHub.Receiver.Beacon.Models;

namespace BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers
{
    /// <summary>
    /// Provides a general contract for parsing raw beacon data into a base telemetry model.
    /// </summary>
    public interface IBeaconTelemeteryGeneralParser
    {
        /// <summary>
        /// Gets the specific type of telemetry that this parser is designed to handle.
        /// </summary>
        Type SupportedType { get; }

        /// <summary>
        /// Parses the provided raw beacon data and returns a base telemetry object.
        /// </summary>
        /// <param name="data">The raw data received from the beacon.</param>
        /// <returns>A telemetry object derived from BeaconTelemetryBase.</returns>
        BeaconTelemetryBase Parse(BeaconRawData data);
    }

    /// <summary>
    /// Provides a specialized, type-safe contract for parsing raw beacon data into a specific telemetry type.
    /// </summary>
    /// <typeparam name="T">The specific type of telemetry, which must inherit from BeaconTelemetryBase.</typeparam>
    public interface IBeaconTelemeteryDetailedParser<out T> : IBeaconTelemeteryGeneralParser where T : BeaconTelemetryBase
    {
        /// <summary>
        /// Parses the provided raw data and returns a strongly-typed telemetry object.
        /// This method hides the general Parse method to provide type safety.
        /// </summary>
        /// <param name="rawData">The raw data received from the beacon.</param>
        /// <returns>A strongly-typed telemetry object of type T.</returns>
        new T Parse(BeaconRawData rawData);
    }
}
