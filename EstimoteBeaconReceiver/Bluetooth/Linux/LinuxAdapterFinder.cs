using Linux.Bluetooth;
using Serilog;

namespace EstimoteBeaconReceiver.Bluetooth.Linux
{
    
    internal class LinuxAdapterFinder : IBleAdapterFinder
    {
        public async Task<IBleAdapter> GetAdapter(string? name = null)
        {
            IReadOnlyList<Adapter>? allAdapters = null;
            try
            {
                allAdapters = await BlueZManager.GetAdaptersAsync();
            }
            catch (Exception ex)
            {
                throw new BleAdapterException("An exception occurred while scanning adapters.", ex);
            }
            if (allAdapters == null ||
                allAdapters.Count == 0)
            {
                throw new BleAdapterException("No adapters found in system.");
            }
            Log.Debug($"Found {allAdapters.Count} BLE adapter(s) in system, {string.Join(", ",allAdapters.Select(x => x.Name))}");
            if (name != null)
            {
                return new BleLinuxAdapter(allAdapters.FirstOrDefault(x => x.Name == name) ?? throw new BleAdapterException($"Cannot find adapter with the specified name: '{name}'."));
            }
            return new BleLinuxAdapter(allAdapters[0]);
        }
    }
}
