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
    internal class TelemetryBExtendedParser : TelemetryBParser, IBeaconTelemeteryDetailedParser<BeaconTelemetryBExtended>
    {
        public override Type SupportedType => typeof(BeaconTelemetryBExtended);
        public new BeaconTelemetryBExtended Parse(BeaconRawData rawData)
        {
            BeaconTelemetryB a = base.Parse(rawData);
            ReadOnlySpan<byte> data = rawData.ServiceData;
            MagneticFieldStrength magneticField = new(
               X_axis: (sbyte)data[10] / 128.0,
               Y_axis: (sbyte)data[11] / 128.0,
               Z_axis: (sbyte)data[12] / 128.0
            );
            var ambientLightUpper = (data[13] & 0b1111_0000) >> 4;
            var ambientLightLower = data[13] & 0b0000_1111;
            Illuminance ambientIlluminance = Illuminance.FromLux(Math.Pow(2, ambientLightUpper) * ambientLightLower * 0.72);
            return new(a, magneticField, ambientIlluminance);
        }
    }
}
