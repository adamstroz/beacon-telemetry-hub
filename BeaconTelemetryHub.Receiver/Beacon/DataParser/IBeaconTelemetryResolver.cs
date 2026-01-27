using BeaconTelemetryHub.Receiver.Beacon.Models;

namespace BeaconTelemetryHub.Receiver.Beacon.DataParser
{
    /// <summary>
    /// Provides functionality for resolving and creating beacon telemetry objects
    /// from raw beacon data.
    /// </summary>
    public interface IBeaconTelemetryResolver
    {
        /// <summary>
        /// Determines the concrete telemetry type based on the provided raw beacon data.
        /// </summary>
        /// <param name="rawData">The raw data received from the beacon.</param>
        /// <returns>The resolved telemetry <see cref="Type"/>.</returns>
        public Type DetermineTelemeteryTypeFromRawData(BeaconRawData rawData);

        /// <summary>
        /// Creates a telemetry instance of the specified type from raw beacon data.
        /// </summary>
        /// <typeparam name="T">The concrete telemetry type to create.</typeparam>
        /// <param name="rawData">The raw data received from the beacon.</param>
        /// <returns>An instance of telemetry of type <typeparamref name="T"/>.</returns>
        public T CreateTelemetry<T>(BeaconRawData rawData) where T : BeaconTelemetryBase;
    }

}
