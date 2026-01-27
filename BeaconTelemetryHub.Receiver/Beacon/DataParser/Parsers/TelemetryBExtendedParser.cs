using BeaconTelemetryHub.Receiver.Beacon.Models;
using UnitsNet;

namespace BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers
{
    // This parser applies to [Location Beacons] beacon type
    public class TelemetryBExtendedParser : TelemetryBParser, IBeaconTelemeteryDetailedParser<BeaconTelemetryBExtended>
    {
        public override Type SupportedType => typeof(BeaconTelemetryBExtended);
        public new BeaconTelemetryBExtended Parse(BeaconRawData rawData)
        {
            // Create telemetry B to extend
            BeaconTelemetryB b = base.Parse(rawData);

            ReadOnlySpan<byte> rawServiceData = rawData.ServiceData;

            // Magnetic field strength
            MagneticFieldStrength magneticField = new(
               X_axis: (sbyte)rawServiceData[10] / 128.0,
               Y_axis: (sbyte)rawServiceData[11] / 128.0,
               Z_axis: (sbyte)rawServiceData[12] / 128.0
            );
            var ambientLightUpper = (rawServiceData[13] & 0b1111_0000) >> 4;
            var ambientLightLower = rawServiceData[13] & 0b0000_1111;
            Illuminance ambientIlluminance = Illuminance.FromLux(Math.Pow(2, ambientLightUpper) * ambientLightLower * 0.72);

            return new(b, magneticField, ambientIlluminance);
        }
    }
}
