using AutoMapper;
using BeaconTelemetryHub.DataContracts.DataStore;
using BeaconTelemetryHub.DataContracts.Models;
using BeaconTelemetryHub.Receiver.Beacon.BeaconFinder;
using BeaconTelemetryHub.Receiver.Beacon.Models;
using BeaconTelemetryHub.Receiver.Bluetooth;
using BeaconTelemetryHub.Receiver.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace BeaconTelemetryHub.Host
{
    /// <summary>
    /// A core background service responsible for orchestrating the beacon discovery process 
    /// and processing incoming telemetry data.
    /// </summary>
    /// <remarks>
    /// This service initializes the BLE adapter, listens for beacon advertisements, 
    /// and send mapped telemetry data to the designated data store.
    /// </remarks>
    /// <param name="beaconFinder">The service used to discover and identify specific beacon protocols.</param>
    /// <param name="bleAdapterBuilder">The builder used to configure and initialize the Bluetooth Low Energy adapter.</param>
    /// <param name="mapper">The AutoMapper instance used to transform raw telemetry into Data Transfer Objects (DTOs).</param>
    /// <param name="beaconDataStore">The central data store for distributing processed telemetry to various sinks.</param>
    /// <param name="hostApplicationLifetime">Service that allows for graceful shutdown and management of the host's lifecycle.</param>
    /// <param name="settings">Configuration options specifically for the beacon receiver module.</param>
    internal class BeaconBackgroundService(IBeaconFinder beaconFinder, 
                                           IBleAdapterBuilder bleAdapterBuilder,
                                           IMapper mapper,
                                           IBeaconDataStore beaconDataStore,
                                           IHostApplicationLifetime hostApplicationLifetime,
                                           IOptions<BeaconTelemetryHubSettings> settings)
                                           : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                Log.Information("Starting beacon data receiver...");
                beaconFinder.BeaconFound += BeaconFinder_BeaconFound;
                IBleAdapter bleAdapter;
                try
                {
                    Log.Debug("Building the ble adapter...");
                    bleAdapter = await bleAdapterBuilder.BuildAdapter(settings.Value.AdapterName);
                }
                catch (Exception ex)
                {
                    Log.Error("Cannot build ble adapter, see exception from details", ex);
                    Environment.ExitCode = (int)ExitCode.Error;
                    return;
                }
                Log.Information("Ble adapter ('{AdapterName}') is selected and ready for work", bleAdapter.Name);
                try
                {
                    using (bleAdapter)
                    {
                        await beaconFinder.Search(bleAdapter, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Exception in searching beacons", ex);
                    Environment.ExitCode = (int)ExitCode.Error;
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error in beacon receiver background service", ex);
                Environment.ExitCode = (int)ExitCode.Error;
            }
            finally
            {
                beaconFinder.BeaconFound -= BeaconFinder_BeaconFound;
                Log.Information("Beacon data receiver background service is stopping...");
                hostApplicationLifetime.StopApplication();
            }
        }
        // This is an event handler, so 'async void' is used to allow asynchronous database calls.
        // All logic is wrapped in a try-catch block because an unhandled exception in an 
        // 'async void' method would terminate the entire application.
        private async void BeaconFinder_BeaconFound(BeaconTelemetryBase telemetryData)
        {
            try
            {
                Log.Debug("Received telemetery data from beacon, beacon Id: '{BeaconIdentifier}', telemetery type: '{Type}', bluetooth device address: '{Address}'",
                            telemetryData.DeviceIdentifier,
                            telemetryData.GetType().Name,
                            telemetryData.DeviceBleAddress);
                switch (telemetryData)
                {
                    case BeaconTelemetryAExtended telemetryAExt:
                        Log.Verbose("Telemetry A Extended found: {Beacon}", telemetryAExt);
                        await beaconDataStore.StoreRssi(mapper.Map<RssiDto>(telemetryAExt));

                        break;
                    case BeaconTelemetryBExtended telemetryBExt:
                        Log.Verbose("Telemetry B Extended found: {Beacon}", telemetryBExt);
                        await beaconDataStore.StoreRssi(mapper.Map<RssiDto>(telemetryBExt));

                        break;
                    case BeaconTelemetryA telemetryA:
                        Log.Verbose("Telemetry A found: {Beacon}", telemetryA);
                        await beaconDataStore.StoreRssi(mapper.Map<RssiDto>(telemetryA));

                        break;
                    case BeaconTelemetryB telemetryB:
                        Log.Verbose("Telemetry B found: {Beacon}", telemetryB);
                        await beaconDataStore.StoreTemperature(mapper.Map<TemperatureDto>(telemetryB));
                        if (telemetryB.BatteryLevel.HasValue &&
                            telemetryB.BatteryVoltage.HasValue)
                        {
                            await beaconDataStore.StoreBattery(mapper.Map<BatteryDto>(telemetryB));
                        }
                        await beaconDataStore.StoreRssi(mapper.Map<RssiDto>(telemetryB));
                        break;
                    default:
                        Log.Warning("Unknown beacon telementry type found: {Beacon}", telemetryData);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,
                          "Cannot store beacon telemetry data from beacon, " +
                          "beacon Id: '{BeaconIdentifier}', " +
                          "telemetery type: '{Type}', " +
                          "bluetooth device address: '{Address}'", 
                          telemetryData.DeviceIdentifier,
                          telemetryData.GetType().Name,
                          telemetryData.DeviceBleAddress);
                Environment.ExitCode = (int)ExitCode.Error;
                hostApplicationLifetime.StopApplication();
            }
        }
    }
}
