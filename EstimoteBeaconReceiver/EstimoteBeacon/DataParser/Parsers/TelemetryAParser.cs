using EstimoteBeaconReceiver.EstimoteBeacon.Models;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace EstimoteBeaconReceiver.EstimoteBeacon.DataParser.Parsers
{
    internal class TelemetryAParser : TelemetryParserBase, IBeaconTelemeteryDetailedParser<BeaconTelemetryA>
    {
        public virtual Type SupportedType => typeof(BeaconTelemetryA);

        public BeaconTelemetryA Parse(BeaconRawData rawData)
        {
            ReadOnlySpan<byte> rawServiceData = rawData.ServiceData;
            if (rawServiceData.Length < 20)
            {
                throw new InvalidOperationException("Invalid service length, the Estimote service data is too short to create Telemetry A.");
            }
            
            // Protocol version
            short protocolVersion = GetProtocolVersion(rawServiceData);

            // Acceleration
            AccelerationData acceleration = new()
            {
                X_axis = Acceleration.FromStandardGravity((sbyte)rawServiceData[10] * 2 / 127.0),
                Y_axis = Acceleration.FromStandardGravity((sbyte)rawServiceData[11] * 2 / 127.0),
                Z_axis = Acceleration.FromStandardGravity((sbyte)rawServiceData[12] * 2 / 127.0),
            };

            // Motion state
            bool isMoving = (rawServiceData[15] & 0b00000011) == 1;

            // Motion state duration
            static TimeSpan ParseMotionStateDuration(byte b)
            {
                int num = b & 0b00111111;
                int unitCode = (b & 0b11000000) >> 6;
                return unitCode switch
                {
                    0 => TimeSpan.FromSeconds(num),
                    1 => TimeSpan.FromMinutes(num),
                    2 => TimeSpan.FromHours(num),
                    3 => TimeSpan.FromDays(num),
                    _ => throw new InvalidOperationException("Invalid motion state duration unit code."),
                };
            }
           
            // Errors
            EstimoteErrorCodes? errors = null;
            if (protocolVersion == 2)
            {
                bool hasFirmwareError = (rawServiceData[15] & 0b00000100) >> 2 == 1;
                bool hasClockError = (rawServiceData[15] & 0b00001000) >> 3 == 1;
                errors = new EstimoteErrorCodes(hasFirmwareError, hasClockError);
            }
            else if (protocolVersion == 1)
            {
                bool hasFirmwareError = (rawServiceData[16] & 0b00000001) == 1;
                bool hasClockError = (rawServiceData[16] & 0b00000010) >> 1 == 1;
                errors = new EstimoteErrorCodes(hasFirmwareError, hasClockError);
            }

            return new BeaconTelemetryA(
                BeaconIdentifier: GetBeaconIdentifier(rawServiceData),
                ProtocolVersion: protocolVersion,
                FoundTimestamp: rawData.FoundTimeStamp,
                BleDeviceAddress: rawData.BleDeviceAddress,
                ErrorCodes: errors,
                Acceleration: acceleration,
                IsMoving: isMoving,
                PreviousMotionStateDuration: ParseMotionStateDuration(rawServiceData[13]),
                CurrentMotionStateDuration: ParseMotionStateDuration(rawServiceData[14])
            );
        }
        

        BeaconTelemetryBase IBeaconTelemeteryGeneralParser.Parse(BeaconRawData rawData)
        {
            return Parse(rawData);
        }
    }
}
