using AutoMapper;
using BeaconTelemetryHub.Database.Context;
using BeaconTelemetryHub.Database.Models;
using BeaconTelemetryHub.DataContracts.DataStore;
using BeaconTelemetryHub.DataContracts.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BeaconTelemetryHub.Database.DataSink
{
    internal class BeaconDatabaseDataSink(IDbContextFactory<BeaconDbContext> dbContextFactory,
                                          IMapper mapper) : IBeaconDataSink
    {
        // Limit concurrency to a single thread at a time for beacon creating operations
        private readonly SemaphoreSlim _beaconSemaphore = new(1, 1);
        public async Task StoreBattery(BatteryDto batteryDto)
        {
            await SaveDataToContext(async (context) =>
            {
                var beacon = await GetOrCreateBeaconAsync(context, batteryDto);
                var battery = mapper.Map<Battery>(batteryDto);
                battery.BeaconId = beacon.Id;
                battery.Beacon = beacon;
                return battery;
            });
        }

        public async Task StoreTemperature(TemperatureDto temperatureDto)
        {
            await SaveDataToContext(async (context) =>
            {
                var beacon = await GetOrCreateBeaconAsync(context, temperatureDto);
                var temperature = mapper.Map<Temperature>(temperatureDto);
                temperature.BeaconId = beacon.Id;
                temperature.Beacon = beacon;
                return temperature;
            });
        }
        public async Task StoreRssi(RssiDto rssiDto)
        {
            await SaveDataToContext(async (context) =>
            {
                var beacon = await GetOrCreateBeaconAsync(context, rssiDto);
                Rssi rssi = mapper.Map<Rssi>(rssiDto);
                rssi.BeaconId = beacon.Id;
                rssi.Beacon = beacon;
                return rssi;
            });
        }
    
        private async Task<Beacon> GetOrCreateBeaconAsync(BeaconDbContext context, DtoBase dto)
        {
            var existing = await context.Beacons.FirstOrDefaultAsync(b => b.DeviceIdentifier == dto.DeviceIdentifier);
            if (existing != null)
            {
                return existing;
            }
            await _beaconSemaphore.WaitAsync();
            try
            {
                existing = await context.Beacons.FirstOrDefaultAsync(b => b.DeviceIdentifier == dto.DeviceIdentifier);
                if (existing != null)
                {
                    return existing;
                }
                var newBeacon = mapper.Map<Beacon>(dto);
                await context.Beacons.AddAsync(newBeacon);
                await context.SaveChangesAsync();
                return newBeacon;
            }
            finally
            {
                _beaconSemaphore.Release();
            }
        }

        private async Task SaveDataToContext<T>(Func<BeaconDbContext, Task<T>> action) where T : BaseEntity
        {
            try
            {
                using var context = await dbContextFactory.CreateDbContextAsync();
                var entity = await action(context);
                await context.Set<T>().AddAsync(entity);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                const int UniqueConstraintViolation = 2627;
                const int DuplicateKeyViolation = 2601;
                var sqlEx = (ex.InnerException as SqlException) ?? (ex.InnerException?.InnerException as SqlException);
                if (sqlEx?.Number == UniqueConstraintViolation ||
                    sqlEx?.Number == DuplicateKeyViolation ||
                    ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
                    ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
                {
                    Log.Warning("A duplicate record was found in the data of type: {Type}- skipping persistence.", typeof(T));
                    return;
                }

                throw new InvalidOperationException("An db update error occurred while persisting beacon data.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An general error occurred while persisting beacon data.", ex);
            }
        }
    }
}