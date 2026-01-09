using EstimoteBeaconReceiver.EstimoteBeacon.DataParser;
using EstimoteBeaconReceiver.EstimoteBeacon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimoteBeaconReceiver.EstimoteBeacon.PacketDataParser
{
    internal interface IBeaconTelemetryResolver
    {
        public Type DetermineTelemeteryTypeFromRawData(BeaconRawData rawData);
        public T CreateTelemetry<T>(BeaconRawData rawData) where T : BeaconTelemetryBase;
    }
}
