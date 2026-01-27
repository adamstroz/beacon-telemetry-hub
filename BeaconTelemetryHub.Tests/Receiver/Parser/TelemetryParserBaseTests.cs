using BeaconTelemetryHub.Receiver.Beacon.DataParser.Parsers;
using BeaconTelemetryHub.Receiver.Beacon.Models;

namespace BeaconTelemetryHub.Tests.Receiver.Parser
{
    [TestClass]
    public class TelemetryParserBaseTests
    {
        // Wrapper Class for testing abstract base class with static protected methods
        private class TestableTelemetryParser : TelemetryParserBase
        {
            public static string PublicGetBeaconIdentifier(ReadOnlySpan<byte> data)
            {
                return GetBeaconIdentifier(data);
            }

            public static byte PublicGetProtocolVersion(ReadOnlySpan<byte> data)
            {
                return GetProtocolVersion(data);
            }

            public static void PublicThrowIfProtocolVersionInvalid(byte protocolVersion)
            {
                ThrowIfProtocolVersionInvalid(protocolVersion);
            }
        }
        // For Beacon Identifier tests
        // Bytes 1, 2, 3, 4, 5, 6, 7, 8 => first half of the identifier of the beacon

        [TestMethod]
        public void GetBeaconIdentifier_ValidData_ReturnsCorrectHexString()
        {
            // Arrange
           
            byte[] data =
            [
                0x00, // Byte 0 (unused in beacon identifier)
                0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x11, 0x22, // Bytes 1-8 (Beacon Identifier)
            ];

            // Act
            var result = TestableTelemetryParser.PublicGetBeaconIdentifier(data);

            // Assert
            Assert.IsNotNull(result, "Beacon identifier is null");
            Assert.AreEqual("AABBCCDDEEFF1122", result, "Wrong beacon identifier returned");
        }

        [TestMethod]
        public void GetBeaconIdentifier_DataLongerThanExpected_ReturnsCorrectHexString()
        {
            // Arrange
            byte[] data = new byte[20];
            data[1] = 0x01;
            data[8] = 0x08;

            // Act
            var result = TestableTelemetryParser.PublicGetBeaconIdentifier(data);

            // Assert
            Assert.AreEqual("0100000000000008", result, "Wrong beacon identifier returned");
        }

        [TestMethod]
        public void GetBeaconIdentifier_DataTooShort_ThrowsInvalidOperationException()
        {
            // Arrange
            // Invalid array size
            byte[] data = new byte[8];

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => _= TestableTelemetryParser.PublicGetBeaconIdentifier(data));
        }

        [TestMethod]
        public void GetBeaconIdentifier_EmptyData_ThrowsInvalidOperationException()
        {
            // Arrange
            byte[] data = [];

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => _ = TestableTelemetryParser.PublicGetBeaconIdentifier(data));
        }

        // For protocol version
        // Byte 0, upper 4 bits => Telemetry protocol version ("0", "1", "2", etc.)
        // Mask: 0b1111_0000
        // Shift  >> 4
        // Example 
        // Input: 0b0010_1001
        // Input & Mask: 0b0010_1001 & 0b1111_0000 = 0b0010_0000
        // Shift: 0b0010_0000 >> 4 = 0b0000_0010
        // Verison = 0x02 (Version 2)

        [TestMethod]
        public void GetProtocolVersion_ValidData_ReturnsCorrectVersion()
        {
            // Arrange
            byte protocolVersionInput = 0b0010_0000;
            byte protocolVersion = 0x02;
            byte[] data = new byte[10];
            data[0] = protocolVersionInput;

            // Act
            var result = TestableTelemetryParser.PublicGetProtocolVersion(data);

            // Assert
            Assert.AreEqual(protocolVersion, result);
        }

        [TestMethod]
        public void GetProtocolVersion_IgnoresLowerBits()
        {
            // Arrange
            // 4 lower bits set to 1, should be ignored
            byte protocolVersionInput = 0b0010_1111;
            byte protocolVersion = 0x02;
            byte[] data = new byte[10];
            data[0] = protocolVersionInput;

            // Act
            var result = TestableTelemetryParser.PublicGetProtocolVersion(data);

            // Assert
            Assert.AreEqual(protocolVersion, result);
        }

        [TestMethod]
        public void GetProtocolVersion_DataTooShort_ThrowsInvalidOperationException()
        {
            // Arrange
            // Array to short (zero size)
            byte[] data = []; 

            // Act
            Assert.ThrowsException<InvalidOperationException>(() => TestableTelemetryParser.PublicGetProtocolVersion(data));
        }

        [TestMethod]
        public void ThrowIfProtocolVersionInvalid_VersionIsSupported_DoesNotThrow()
        {
            // Arrange
            byte validVersion = BeaconTelemetryBase.SupportedProtocolMaximumVersion;

            // Act
            TestableTelemetryParser.PublicThrowIfProtocolVersionInvalid(validVersion);
        }

        [TestMethod]
        public void ThrowIfProtocolVersionInvalid_VersionIsLowerThanMax_DoesNotThrow()
        {
            // Arrange
            byte validVersion = 0;  

            // Act
            TestableTelemetryParser.PublicThrowIfProtocolVersionInvalid(validVersion);
        }

        [TestMethod]
        public void ThrowIfProtocolVersionInvalid_VersionTooHigh_ThrowsNotSupportedException()
        {
            // Arrange
            byte invalidVersion = (byte)(BeaconTelemetryBase.SupportedProtocolMaximumVersion + 1);

            // Act
            Assert.ThrowsException<NotSupportedException>(() => TestableTelemetryParser.PublicThrowIfProtocolVersionInvalid(invalidVersion));
        }
    }
}
