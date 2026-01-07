using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimoteBeaconReceiver.Bluetooth
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
    internal interface IBleAdapterBuilder
    {
        public Task<IBleAdapter> GetAdapter(string? name = null);
    }
}
