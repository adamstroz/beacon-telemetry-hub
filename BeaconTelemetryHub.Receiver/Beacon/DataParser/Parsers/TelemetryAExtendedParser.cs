using BeaconTelemetryHub.Receiver.Beacon.Models;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers
{
    // This parser applies to [Location Beacons] beacon type
    internal class TelemetryAExtendedParser : TelemetryAParser, IBeaconTelemeteryDetailedParser<BeaconTelemetryAExtended>
    {
        public override Type SupportedType => typeof(BeaconTelemetryAExtended);
        public new BeaconTelemetryAExtended Parse(BeaconRawData rawData)
        {
            BeaconTelemetryA a = base.Parse(rawData);
            ReadOnlySpan<byte> rawServiceData = rawData.ServiceData;
            // GPIO pins
            GpioPins gpioPins = new(
                Pin0: (rawServiceData[15] & 0b00010000) != 0,
                Pin1: (rawServiceData[15] & 0b00100000) != 0,
                Pin2: (rawServiceData[15] & 0b01000000) != 0,
                Pin3: (rawServiceData[15] & 0b10000000) != 0
            );
            // Atmospheric pressure
            if (a.ProtocolVersion == 2)
            {
               throw new NotSupportedException("Telemetry A Extended with Protocol Version 2 is not supported.");
            }
            uint pressureRaw = BinaryPrimitives.ReadUInt32LittleEndian(rawServiceData.Slice(16, 4));
            return new(a, gpioPins, Pressure.FromPascals(pressureRaw / 256.0));
        }
    }
}
