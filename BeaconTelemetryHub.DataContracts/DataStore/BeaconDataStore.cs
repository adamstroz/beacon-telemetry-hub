using BeaconTelemetryHub.DataContracts.Models;

namespace BeaconTelemetryHub.DataContracts.DataStore
{
    // TODO: Add selecting sink destination from configuration (eq. only database, only csv, all)
    /// <summary>
    /// Orchestrates the distribution of beacon telemetry data to all multiple registered sinks.
    /// This class implements the <see cref="IBeaconDataStore"/> interface and acts as a 
    /// composite dispatcher, ensuring that every received data packet is processed by 
    /// all available storage providers.
    /// </summary>
    /// <param name="sinks">A collection of data sinks that will receive the telemetry data. Typically provided by DI container</param>
    internal class BeaconDataStore(IEnumerable<IBeaconDataSink> sinks) : IBeaconDataStore
    {
        public async Task StoreBattery(BatteryDto batteryInfo)
        {
            foreach (var sink in sinks)
            {
                await sink.StoreBattery(batteryInfo);
            }
        }

        public async Task StoreRssi(RssiDto rssiDto)
        {
            foreach (var sink in sinks)
            {
                await sink.StoreRssi(rssiDto);
            }
        }

        public async Task StoreTemperature(TemperatureDto temperatureData)
        {
            foreach (var sink in sinks) 
            { 
                await sink.StoreTemperature(temperatureData);
            }
        }
    }
}
