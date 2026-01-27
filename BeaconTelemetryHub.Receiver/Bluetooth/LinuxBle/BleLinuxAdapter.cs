using BeaconTelemetryHub.Receiver.Bluetooth.Models;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Serilog;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace BeaconTelemetryHub.Receiver.Bluetooth.LinuxBle
{
    internal class BleLinuxAdapter(Adapter adapter) : IBleAdapter
    {
        private readonly TimeSpan _minimalScanDuration = TimeSpan.FromSeconds(1);
        private readonly SemaphoreSlim _controllSemaphore = new(1,1);
        private bool _disposed = false;
        private readonly ImmutableDictionary<string, object> _filters = new Dictionary<string, object>
        {
            // Use only BLE layer
            { "Transport", "le" }
        }.ToImmutableDictionary();

        public string Name
        {
            get
            {
                if (adapter == null)
                {
                    throw new NullReferenceException("Adapter not set");
                }
                return adapter.Name;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                adapter.Dispose();
            }

            _disposed = true;
        }

        public async Task<IReadOnlyDictionary<BleDeviceAddress, BleDeviceAdvertisementPacket>> DiscoveryAdvertisementPackets(TimeSpan scanDuration,
                                                                                                                             CancellationToken cancellationToken)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(scanDuration, _minimalScanDuration, nameof(scanDuration));
            if (adapter == null)
            {
                throw new NullReferenceException("Adapter not set");
            }
            Dictionary<BleDeviceAddress, BleDeviceAdvertisementPacket> discoveredPackets = [];
            await _controllSemaphore.WaitAsync(cancellationToken);
            try
            {
                using (await adapter.WatchDevicesAddedAsync(async device =>
                {
                    Device1Properties devProps;
                    try
                    {
                        devProps = await device.GetAllAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.Warning($"Cannot get properites from found device: '{device.ObjectPath}'", ex);
                        return;
                    }
         
                    try
                    {
                        BleDeviceAdvertisementPacket packet = new(new(devProps.Address), devProps.RSSI, devProps.ServiceData?.ToFrozenDictionary());
                        discoveredPackets.Add(new(devProps.Address), packet);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning("Cannot add a found Ble packet to results", ex);
                    }
                }))
                {
                    try
                    {
                        await adapter.SetDiscoveryFilterAsync(_filters);
                        await adapter.StartDiscoveryAsync();
                        await Task.Delay(scanDuration, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // Task canceled 
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Cannot start Ble packet discovering", ex);
                    }
                    finally
                    {
                        try
                        {
                            await adapter.StopDiscoveryAsync();
                        }
                        catch (Exception ex)
                        {
                            Log.Warning("Cannot stop Ble packet discovering", ex);
                        }
                    } 
                }
            }
            finally
            {
                _controllSemaphore.Release();
            }
            return discoveredPackets;
        }
    }
}
