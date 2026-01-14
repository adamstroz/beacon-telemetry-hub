namespace BeaconTelemetryHub.Receiver.Bluetooth
{
    /// <summary>
    /// Builds and provides Bluetooth Low Energy (BLE) adapter instances.
    /// </summary>
    public interface IBleAdapterBuilder
    {
        /// <summary>
        /// Gets a BLE adapter instance by name or first available in system.
        /// </summary>
        /// <param name="name">The name of the BLE adapter to retrieve, or null for the default adapter.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the BLE adapter instance.
        /// </returns>
        public Task<IBleAdapter> GetAdapter(string? name = null);
    }
}
