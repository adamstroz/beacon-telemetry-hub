using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaconTelemetryHub.Receiver.Bluetooth
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
    /// <summary>
    /// Builds and provides Bluetooth Low Energy (BLE) adapter instances.
    /// </summary>
    internal interface IBleAdapterBuilder
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
