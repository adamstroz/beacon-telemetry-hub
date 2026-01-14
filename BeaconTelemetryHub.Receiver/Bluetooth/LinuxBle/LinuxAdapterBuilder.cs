using Linux.Bluetooth;
using Serilog;

namespace BeaconTelemetryHub.Receiver.Bluetooth.LinuxBle
{
    internal delegate BleLinuxAdapterWrapper GetBleLinuxAdapterWrapperDelegate();
    internal class LinuxAdapterBuilder(GetBleLinuxAdapterWrapperDelegate getWrapper) : IBleAdapterBuilder
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
                throw new InvalidOperationException("An exception occurred while scanning adapters.", ex);
            }
            if (allAdapters == null ||
                allAdapters.Count == 0)
            {
                throw new InvalidOperationException("No adapters found in system.");
            }
            Log.Debug($"Found {allAdapters.Count} BLE adapter(s) in system, {string.Join(", ",allAdapters.Select(x => x.Name))}");
            Adapter selectedAdapter;
            if (name != null)
            {
                selectedAdapter = allAdapters.FirstOrDefault(x => x.Name == name) ?? throw new KeyNotFoundException($"Cannot find adapter with the specified name: '{name}'.");
            }
            else
            {
                selectedAdapter = allAdapters[0];
            }
            BleLinuxAdapterWrapper wrapper = getWrapper.Invoke();
            wrapper.SetupWrapper(selectedAdapter);
            return wrapper;
        }
    }
}
