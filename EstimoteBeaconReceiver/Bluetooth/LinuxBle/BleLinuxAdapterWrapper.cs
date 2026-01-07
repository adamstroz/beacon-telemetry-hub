using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using Serilog;
using System;
using System.Collections.Frozen;
using System.Collections.Immutable;
using Tmds.DBus;
namespace EstimoteBeaconReceiver.Bluetooth.LinuxBle
{
    internal class BleLinuxAdapterWrapper : IBleAdapter, IDisposable
    {
        private readonly SemaphoreSlim _controllSemaphore = new(1,1);
        private Adapter? _adapter = null;
        private readonly List<Device> _discoveredDevices = [];
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
                _adapter.PoweredOff += PoweredOff;
            }
            catch
            {
                
            }
            finally
            {
                _controllSemaphore.Release();
            }
        }
        private Task PoweredOff(Adapter sender, BlueZEventArgs eventArgs)
        {
            throw new NotImplementedException();
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
                        _adapter.PoweredOff -= PoweredOff;
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
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(scanDuration, TimeSpan.Zero, nameof(scanDuration));
            await _controllSemaphore.WaitAsync(cancellationToken);
            Dictionary<BleDeviceAddress, BleDeviceAdvertisementPacket> packets = [];
            try
            {
              
                if (_adapter == null)
                {
                    throw new NullReferenceException("Adapter is null!");
                }
                using (await _adapter.WatchDevicesAddedAsync(async device =>
                {
                    Device1Properties devProps = await device.GetAllAsync();
                    BleDeviceAdvertisementPacket packet = new(new(devProps.Address), devProps.RSSI, devProps.ServiceData?.ToFrozenDictionary());
                    Log.Debug($"Ble device aedvertising packet detected: {packet}");
                    packets.Add(new(devProps.Address), packet);

                }))
                {
                    try
                    {
                        await _adapter.SetDiscoveryFilterAsync(_filters);
                        await _adapter.StartDiscoveryAsync();
                        await Task.Delay(scanDuration, cancellationToken);
                        await _adapter.StopDiscoveryAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("An exception occurred during BLE advertisement packets discovery.", ex);
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
