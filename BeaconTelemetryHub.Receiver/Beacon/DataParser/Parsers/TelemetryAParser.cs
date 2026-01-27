using BeaconTelemetryHub.Receiver.Beacon.Models;
using UnitsNet;

namespace BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers
{
    public class TelemetryAParser : TelemetryParserBase, IBeaconTelemeteryDetailedParser<BeaconTelemetryA>
    {
        public const int ExpectedServiceDataLengthForTelemetryTelemetryA = 17;
        public const byte TelemetryTypeAValue = 0x00;
        public virtual Type SupportedType => typeof(BeaconTelemetryA);

        public BeaconTelemetryA Parse(BeaconRawData rawData)
        {
            ReadOnlySpan<byte> rawServiceData = rawData.ServiceData;    
            
            // Protocol version
            var protocolVersion = GetProtocolVersion(rawServiceData);
            ThrowIfProtocolVersionInvalid(protocolVersion);

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
            BeaconErrorCodes? errors = null;
            if (protocolVersion == 2)
            {
                bool hasFirmwareError = (rawServiceData[15] & 0b00000100) >> 2 == 1;
                bool hasClockError = (rawServiceData[15] & 0b00001000) >> 3 == 1;
                errors = new BeaconErrorCodes(hasFirmwareError, hasClockError);
            }
            else if (protocolVersion == 1)
            {
                bool hasFirmwareError = (rawServiceData[16] & 0b00000001) == 1;
                bool hasClockError = (rawServiceData[16] & 0b00000010) >> 1 == 1;
                errors = new BeaconErrorCodes(hasFirmwareError, hasClockError);
            }

            return new BeaconTelemetryA(BeaconIdentifier: GetBeaconIdentifier(rawServiceData),
                                        ProtocolVersion: protocolVersion,
                                        FoundTimestamp: rawData.FoundTimeStamp,
                                        BleDeviceAddress: rawData.BleDeviceAddress,
                                        ErrorCodes: errors,
                                        Acceleration: acceleration,
                                        IsMoving: isMoving,
                                        Rssi: rawData.Rssi,
                                        PreviousMotionStateDuration: ParseMotionStateDuration(rawServiceData[13]),
                                        CurrentMotionStateDuration: ParseMotionStateDuration(rawServiceData[14]));
        }
        
        BeaconTelemetryBase IBeaconTelemeteryGeneralParser.Parse(BeaconRawData rawData)
        {
            return Parse(rawData);
        }
    }
}
