using UnitsNet;
using BeaconTelemetryHub.Receiver.Beacon.Models;

namespace BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers
{
    public class TelemetryBParser : TelemetryParserBase, IBeaconTelemeteryDetailedParser<BeaconTelemetryB>
    {
        public virtual Type SupportedType => typeof(BeaconTelemetryB);

        public BeaconTelemetryB Parse(BeaconRawData rawData)
        {
            ReadOnlySpan<byte> data = rawData.ServiceData;

            // Protocol version
            var protocolVersion = GetProtocolVersion(data);

            // Uptime
            var uptimeUnitCode = (data[15] & 0b0011_0000) >> 4;
            TimeSpan uptime = uptimeUnitCode switch
            {
                0 => TimeSpan.FromSeconds((data[15] & 0b0000_1111) << 8 | data[14]),
                1 => TimeSpan.FromMinutes((data[15] & 0b0000_1111) << 8 | data[14]),
                2 => TimeSpan.FromHours((data[15] & 0b0000_1111) << 8 | data[14]),
                3 => TimeSpan.FromDays((data[15] & 0b0000_1111) << 8 | data[14]),
                _ => throw new InvalidOperationException("Invalid uptime unit code")
            };

            // Temperature
            int temperatureRawValue =
                (data[17] & 0b0000_0011) << 10 |
                data[16] << 2 |
                (data[15] & 0b1100_0000) >> 6;

            if (temperatureRawValue > 2047)
                temperatureRawValue -= 4096;

            var temperature = Temperature.FromDegreesCelsius(temperatureRawValue / 16.0);

            // Battery voltage
            int batteryVoltageRaw = data[18] << 6 | (data[17] & 0b1111_1100) >> 2;
            int? batteryVoltageIn_mV = batteryVoltageRaw == 0b11_1111_1111_1111 ? null : batteryVoltageRaw;
            ElectricPotential? batteryVoltage = batteryVoltageIn_mV == null ? null: ElectricPotential.FromMillivolts(batteryVoltageIn_mV.Value);

            // Errors
            BeaconErrorCodes? errors = null;
            if (protocolVersion == 0)
            {
                errors = new BeaconErrorCodes
                {
                    HasFirmwareError = (data[19] & 0b0000_0001) != 0,
                    HasClockError = (data[19] & 0b0000_0010) != 0
                };
            }

            // Battery level
            Ratio? batteryLevel = null;
            var rawBatteryLevel = data[19];
            if (rawBatteryLevel != 0xFF)
            {
                batteryLevel = Ratio.FromPercent(rawBatteryLevel);
            }
               
            return new BeaconTelemetryB(
                        UpTime: uptime,
                        Temperature: temperature,
                        BatteryLevel: batteryLevel,
                        BatteryVoltage: batteryVoltage,
                        BeaconIdentifier: GetBeaconIdentifier(data),
                        ProtocolVersion: protocolVersion,
                        FoundTimestamp: rawData.FoundTimeStamp,
                        BleDeviceAddress: rawData.BleDeviceAddress,
                        ErrorCodes: errors
            );
        }

        BeaconTelemetryBase IBeaconTelemeteryGeneralParser.Parse(BeaconRawData rawData)
        {
            return Parse(rawData);
        }
    }
}
