using Linux.Bluetooth;
using Serilog;

namespace EstimoteBeaconReceiver.BluetoothLowEnergy
{
    public class BleAdapterException : Exception
    {
        public BleAdapterException()
        {
        }

        public BleAdapterException(string? message) : base(message)
        {
        }

        public BleAdapterException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
    internal static class AdapterHelper
    {
        public static async Task<Adapter> GetAdapter(string? name = null)
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
                return allAdapters.FirstOrDefault(x => x.Name == name) ?? throw new BleAdapterException($"Cannot find adapter with the specified name: '{name}'.");
            }
            return allAdapters[0];
        }
    }
}
