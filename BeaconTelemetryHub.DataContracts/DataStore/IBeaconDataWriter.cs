using BeaconTelemetryHub.DataContracts.Models;

namespace BeaconTelemetryHub.DataContracts.DataStore
{
    /// <summary>
    /// Defines the contract for writing beacon telemetry data to a storage provider (like database, csv writer etc.).
    /// </summary>
    public interface IBeaconDataWriter
    {
        /// <summary>
        /// Persists temperature data received from a beacon.
        /// </summary>
        /// <param name="temperatureDto">The temperature telemetry data transfer object.</param>
        /// <returns>A task representing the asynchronous storage operation.</returns>
        public Task StoreTemperature(TemperatureDto temperatureDto);

        /// <summary>
        /// Persists battery status information received from a beacon.
        /// </summary>
        /// <param name="batteryDto">The battery info data transfer object.</param>
        /// <returns>A task representing the asynchronous storage operation.</returns>
        public Task StoreBattery(BatteryDto batteryDto);

        /// <summary>
        /// Persists signal strength (RSSI) data received from a beacon.
        /// </summary>
        /// <param name="rssiDto">The signal strength data transfer object.</param>
        /// <returns>A task representing the asynchronous storage operation.</returns>
        public Task StoreRssi(RssiDto rssiDto);
    }
}
