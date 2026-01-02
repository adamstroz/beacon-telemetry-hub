using Linux.Bluetooth;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimoteBeaconReceiver.Bluetooth.Linux
{
    internal class BleLinuxAdapter : IBleAdapter
    {
        private readonly SemaphoreSlim _controllSemaphore = new(1,1);
        private readonly Adapter _adapter;
        private readonly ImmutableDictionary<string, object> _filters = new Dictionary<string, object>
        {
            // Use only BLE layer
            { "Transport", "le" },
            // Allow reporting duplicate Adv packets
            { "DuplicateData", true }  
        }.ToImmutableDictionary();
        public BleLinuxAdapter(Adapter adapter)
        {
            _adapter = adapter;
            _adapter.DeviceFound += DeviceFound;
            _adapter.PoweredOff += PoweredOff;
        }

        private Task PoweredOff(Adapter sender, BlueZEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        private Task DeviceFound(Adapter sender, DeviceFoundEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        public async Task StartDiscoveryAsync()
        {
            await _controllSemaphore.WaitAsync();
            try
            {
                await _adapter.SetDiscoveryFilterAsync(_filters);
                await _adapter.StartDiscoveryAsync();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _controllSemaphore.Release();
            }
        

        }

        public async Task StopDiscoveryAsync()
        {
            await _controllSemaphore.WaitAsync();
            try
            {
                await _adapter.StopDiscoveryAsync();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                _controllSemaphore.Release();
            }
        }
    }
}
