using BeaconTelemetryHub.Receiver.Beacon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers
{
    internal interface IBeaconTelemeteryGeneralParser
    {
        Type SupportedType { get; }
        BeaconTelemetryBase Parse(BeaconRawData data);
    }
    internal interface IBeaconTelemeteryDetailedParser<out T> : IBeaconTelemeteryGeneralParser where T : BeaconTelemetryBase
    {
        new T Parse(BeaconRawData rawData);
    }
}
