using BeaconTelemetryHub.Receiver.Bluetooth.Models;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Serilog;
using System;
using System.Collections.Frozen;
using System.Collections.Immutable;
using Tmds.DBus;
namespace BeaconTelemetryHub.Receiver.Bluetooth.LinuxBle
{
    internal class BleLinuxAdapterWrapper : IBleAdapter, IDisposable
    {
        private readonly TimeSpan _minimalScanDuration = TimeSpan.FromSeconds(1);
        private readonly SemaphoreSlim _controllSemaphore = new(1,1);
        private Adapter? _adapter = null;
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
                if (_adapter == null)
                {
                    throw new NullReferenceException("Adapter not set!");
                }
                return _adapter.Name;
            }
        }
 
        public void SetupWrapper(Adapter adapter)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _controllSemaphore.Wait();
            try
            {
                
                if (_adapter != null)
                {
                    // TODO: Implement adapter change 
                    throw new InvalidOperationException("Adapter can be set only once.");
                }
                _adapter = adapter;
            }
            catch (Exception ex) 
            {
                throw new InvalidOperationException("Ble wrapper setup exception", ex);
            }
            finally
            {
                _controllSemaphore.Release();
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
                _controllSemaphore.Wait();
                try
                {
                    if (_adapter != null)
                    { 
                        _adapter.Dispose();
                        _adapter = null;
                    }
                }
                finally
                {
                    _controllSemaphore.Release();
                    _controllSemaphore.Dispose();
                }
            }

            _disposed = true;
        }

        public async Task<IReadOnlyDictionary<BleDeviceAddress, BleDeviceAdvertisementPacket>> DiscoveryAdvertisementPackets(TimeSpan scanDuration, CancellationToken cancellationToken)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(scanDuration, _minimalScanDuration, nameof(scanDuration));
            Dictionary<BleDeviceAddress, BleDeviceAdvertisementPacket> packets = [];
            await _controllSemaphore.WaitAsync(cancellationToken);
            try
            {
                if (_adapter == null)
                {
                    throw new NullReferenceException("Adapter is not set!");
                }
                using (await _adapter.WatchDevicesAddedAsync(async device =>
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
                        packets.Add(new(devProps.Address), packet);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning("Cannot add a found Ble packet to results", ex);
                    }
                }))
                {
                    try
                    {
                        await _adapter.SetDiscoveryFilterAsync(_filters);
                        await _adapter.StartDiscoveryAsync();
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
                            await _adapter.StopDiscoveryAsync();
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
            return packets;
        }
    }
}
