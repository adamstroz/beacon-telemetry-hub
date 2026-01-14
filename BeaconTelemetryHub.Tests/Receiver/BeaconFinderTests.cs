using BeaconTelemetryHub.Receiver.Beacon.BeaconFinder;
using BeaconTelemetryHub.Receiver.Beacon.DataParser;
using BeaconTelemetryHub.Receiver.Beacon.Models;
using BeaconTelemetryHub.Receiver.Bluetooth;
using BeaconTelemetryHub.Receiver.Bluetooth.Models;
using BeaconTelemetryHub.Receiver.Settings;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeaconTelemetryHub.Tests.Receiver
{
    [TestClass]
    public class BeaconFinderTests
    {
        private Mock<IBeaconTelemetryResolver> _resolverMock = null!;
        private Mock<IOptions<BeaconReceiverSettings>> _settingsMock = null!;
        private Mock<IBleAdapter> _adapterMock = null!;
        private BeaconReceiverSettings _settings = null!;
        private BeaconFinder _sut = null!;

        [TestInitialize]
        public void Setup()
        {
            _resolverMock = new Mock<IBeaconTelemetryResolver>();
            _adapterMock = new Mock<IBleAdapter>();
            _settingsMock = new Mock<IOptions<BeaconReceiverSettings>>();

            // Domyślne poprawne ustawienia
            _settings = new BeaconReceiverSettings
            {
                ScanInterval = TimeSpan.FromSeconds(2),
                ScanDuration = TimeSpan.FromSeconds(1),
                EstimoteServiceUUID = "f9a",
                EstimoteTelemetryPacketTypeId = 0x01
            };

            _settingsMock.Setup(x => x.Value).Returns(_settings);

            _sut = new BeaconFinder(_resolverMock.Object, _settingsMock.Object);
        }

        [TestMethod]
        public async Task Search_ThrowsArgumentNullException_WhenBleAdapterIsNull()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _sut.Search(null!, CancellationToken.None));
        }

        [TestMethod]
        public async Task Search_ThrowsInvalidOperationException_WhenScanIntervalIsTooSmall()
        {
            // Arrange
            _settings.ScanInterval = TimeSpan.FromMilliseconds(500);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _sut.Search(_adapterMock.Object, CancellationToken.None));

            Assert.AreEqual("Scan interval cannot be less than minimal time.", ex.Message);
        }

        [TestMethod]
        public async Task Search_ThrowsInvalidOperationException_WhenScanningFails()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();

            _adapterMock.Setup(x => x.DiscoveryAdvertisementPackets(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Bluetooth hardware error"));

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _sut.Search(_adapterMock.Object, tokenSource.Token));

            Assert.AreEqual("Failed to scan for advertisement packets.", ex.Message);
        }

        [TestMethod]
        public async Task Search_StopsLooping_WhenCancellationRequested()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();

            _adapterMock.Setup(x => x.DiscoveryAdvertisementPackets(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<BleDeviceAddress, BleDeviceAdvertisementPacket>());

            tokenSource.CancelAfter(500);

            // Act
            await _sut.Search(_adapterMock.Object, tokenSource.Token);

            // Assert
            _adapterMock.Verify(x => x.DiscoveryAdvertisementPackets(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }
    }
}
