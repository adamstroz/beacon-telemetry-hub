using BeaconTelemetryHub.DataContracts.DataStore;
using BeaconTelemetryHub.DataContracts.Models;
using Moq;

namespace BeaconTelemetryHub.Tests.DataContracts
{
    [TestClass]
    public class BeaconDataStoreTests
    {
        private Mock<IBeaconDataSink> _sinkMock1 = null!;
        private Mock<IBeaconDataSink> _sinkMock2 = null!;
        private List<IBeaconDataSink> _sinksList = null!;
        private BeaconDataStore _sut = null!;

        [TestInitialize]
        public void Setup()
        {
            _sinkMock1 = new Mock<IBeaconDataSink>();
            _sinkMock2 = new Mock<IBeaconDataSink>();
            _sinksList = [_sinkMock1.Object, _sinkMock2.Object];

            _sut = new BeaconDataStore(_sinksList);
        }

        [TestMethod]
        public async Task StoreBattery_WithMultipleSinks_ShouldCallStoreBatteryOnAllSinks()
        {
            // Arrange
            var batteryDto = new BatteryDto(0, 0, string.Empty, string.Empty, DateTimeOffset.Now); 

            // Act
            await _sut.StoreBattery(batteryDto);

            // Assert
            _sinkMock1.Verify(x => x.StoreBattery(batteryDto), Times.Once, "Sink 1 should be called once.");
            _sinkMock2.Verify(x => x.StoreBattery(batteryDto), Times.Once, "Sink 2 should be called once.");
        }

        [TestMethod]
        public async Task StoreRssi_WithMultipleSinks_ShouldCallStoreRssiOnAllSinks()
        {
            // Arrange
            var rssiDto = new RssiDto(0, string.Empty, string.Empty, DateTimeOffset.Now);

            // Act
            await _sut.StoreRssi(rssiDto);

            // Assert
            _sinkMock1.Verify(x => x.StoreRssi(rssiDto), Times.Once);
            _sinkMock2.Verify(x => x.StoreRssi(rssiDto), Times.Once);
        }

        [TestMethod]
        public async Task StoreTemperature_WithMultipleSinks_ShouldCallStoreTemperatureOnAllSinks()
        {
            // Arrange
            var tempDto = new TemperatureDto(0, string.Empty, string.Empty, DateTimeOffset.Now);

            // Act
            await _sut.StoreTemperature(tempDto);

            // Assert
            _sinkMock1.Verify(x => x.StoreTemperature(tempDto), Times.Once);
            _sinkMock2.Verify(x => x.StoreTemperature(tempDto), Times.Once);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(5)]
        public async Task StoreBattery_WithVaryingSinkCounts_ShouldWorkCorrectly(int sinkCount)
        {
            // Arrange
            var mocks = new List<Mock<IBeaconDataSink>>();
            var sinks = new List<IBeaconDataSink>();

            for (int i = 0; i < sinkCount; i++)
            {
                var mock = new Mock<IBeaconDataSink>();
                mocks.Add(mock);
                sinks.Add(mock.Object);
            }

            var localSut = new BeaconDataStore(sinks);
            var dto = new BatteryDto(0, 0, string.Empty, string.Empty, DateTimeOffset.Now);

            // Act
            await localSut.StoreBattery(dto);

            // Assert
            foreach (var mock in mocks)
            {
                mock.Verify(x => x.StoreBattery(dto), Times.Once);
            }
        }

        [TestMethod]
        public async Task StoreBattery_WhenInputDtoIsNull_ShouldPassNullToSinks()
        {
            // Arrange
            BatteryDto nullDto = null!;

            // Act
            await _sut.StoreBattery(nullDto);

            // Assert
            _sinkMock1.Verify(x => x.StoreBattery(null!), Times.Once);
        }

        [TestMethod]
        public async Task Constructor_WhenSinksCollectionIsNull_ShouldThrowNullReferenceException_OnMethodCall()
        {
            // Arrange
            var sutUnsafe = new BeaconDataStore(null!);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NullReferenceException>(async () =>
            {
                await sutUnsafe.StoreBattery(new BatteryDto(0, 0, string.Empty, string.Empty, DateTimeOffset.Now));
            });
        }

        [TestMethod]
        public async Task StoreBattery_WhenFirstSinkThrows_ShouldStopProcessingAndPropagateException()
        {
            // Arrange
            string exceptionMessage = "Data sink failure";
            var exceptionToThrow = new InvalidOperationException(exceptionMessage);

            _sinkMock1.Setup(x => x.StoreBattery(It.IsAny<BatteryDto>()))
                      .ThrowsAsync(exceptionToThrow);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            {
                await _sut.StoreBattery(new BatteryDto(0, 0, string.Empty, string.Empty, DateTimeOffset.Now));
            });

            // Assert
            Assert.AreEqual(exceptionMessage, ex.Message);

            
            _sinkMock2.Verify(x => x.StoreBattery(It.IsAny<BatteryDto>()), Times.Never,
                "Second sink should not be called if the first one throws.");
        }
    }
}
