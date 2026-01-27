using Linux.Bluetooth;
using Serilog;

namespace BeaconTelemetryHub.Receiver.Bluetooth.LinuxBle
{
    internal class LinuxAdapterBuilder : IBleAdapterBuilder
    {
        public async Task<IBleAdapter> BuildAdapter(string? name = null)
        {
            IReadOnlyList<Adapter>? allAdapters = null;
            try
            {
                allAdapters = await BlueZManager.GetAdaptersAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An exception occurred while scanning adapters.", ex);
            }
            if (allAdapters == null ||
                allAdapters.Count == 0)
            {
                throw new InvalidOperationException("No adapters found in system.");
            }
            Log.Debug($"Found {allAdapters.Count} BLE adapter(s) in system, {string.Join(", ",allAdapters.Select(x => x.Name))}");
            Adapter selectedAdapter;
            if (string.IsNullOrWhiteSpace(name))
            {
                Log.Debug("Adapter name provided, attempting to find '{AdapterName}' in the system", name);
                selectedAdapter = allAdapters.FirstOrDefault(x => x.Name == name) ?? throw new KeyNotFoundException($"Cannot find adapter with the specified name: '{name}'.");
            }
            else
            {
                Log.Debug("Adapter name is not set, using the first found adapter");
                selectedAdapter = allAdapters[0];
            }
            return new BleLinuxAdapter(selectedAdapter);
        }
    }
}
