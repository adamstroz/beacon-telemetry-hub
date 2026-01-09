using EstimoteBeaconReceiver.Bluetooth;
using EstimoteBeaconReceiver.EstimoteBeacon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using UnitsNet;
using Serilog;

namespace EstimoteBeaconReceiver.EstimoteBeacon.DataParser
{
    internal class TelemetryBParser : TelemetryParserBase, IBeaconTelemeteryDetailedParser<BeaconTelemetryB>
    {
        public Type SupportedType => typeof(BeaconTelemetryB);

        public BeaconTelemetryB Parse(BeaconRawData rawData)
        {
            ReadOnlySpan<byte> data = rawData.ServiceData;
            var protocolVersion = GetProtocolVersion(data);
            ThrowIfProtocolVersionInvalid(protocolVersion);

            var magneticField = new MagneticFieldStrength(
                X_axis: (sbyte)data[10] / 128.0,
                Y_axis: (sbyte)data[11] / 128.0,
                Z_axis: (sbyte)data[12] / 128.0
            );

            Illuminance? ambientIlluminance = null;
            if (data[13] != 0xFF)
            {

                var ambientLightUpper = (data[13] & 0b1111_0000) >> 4;
                var ambientLightLower = data[13] & 0b0000_1111;
                ambientIlluminance = Illuminance.FromLux(Math.Pow(2, ambientLightUpper) * ambientLightLower * 0.72);
            }

            var uptimeUnitCode = (data[15] & 0b0011_0000) >> 4;
            TimeSpan uptime = uptimeUnitCode switch
            {
                0 => TimeSpan.FromSeconds(((data[15] & 0b0000_1111) << 8) | data[14]),
                1 => TimeSpan.FromMinutes(((data[15] & 0b0000_1111) << 8) | data[14]),
                2 => TimeSpan.FromHours(((data[15] & 0b0000_1111) << 8) | data[14]),
                3 => TimeSpan.FromDays(((data[15] & 0b0000_1111) << 8) | data[14]),
                _ => throw new InvalidOperationException("Invalid uptime unit code")
            };

            int temperatureRawValue =
                ((data[17] & 0b0000_0011) << 10) |
                (data[16] << 2) |
                ((data[15] & 0b1100_0000) >> 6);

            if (temperatureRawValue > 2047)
                temperatureRawValue -= 4096;

            var temperature = Temperature.FromDegreesCelsius(temperatureRawValue / 16.0);

            int batteryVoltageRaw = (data[18] << 6) | ((data[17] & 0b1111_1100) >> 2);
            int? batteryVoltageIn_mV = batteryVoltageRaw == 0b11_1111_1111_1111 ? null : batteryVoltageRaw;
            ElectricPotential? batteryVoltage = batteryVoltageIn_mV == null ? null: ElectricPotential.FromMillivolts(batteryVoltageIn_mV.Value);

            EstimoteErrorCodes? errors = null;
            if (protocolVersion == 0)
            {
                errors = new EstimoteErrorCodes
                {
                    HasFirmwareError = (data[19] & 0b0000_0001) != 0,
                    HasClockError = (data[19] & 0b0000_0010) != 0
                };
            }

            Ratio? batteryLevel = null;
            var rawBatteryLevel = data[19];
            if (rawBatteryLevel != 0xFF)
            {
                batteryLevel = Ratio.FromPercent(rawBatteryLevel);
            }
               

            return new BeaconTelemetryB(
                MagneticFieldStrength: magneticField,
                AmbientLightLevel: ambientIlluminance,
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
