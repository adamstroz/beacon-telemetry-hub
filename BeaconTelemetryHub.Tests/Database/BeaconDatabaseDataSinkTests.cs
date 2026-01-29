using AutoMapper;
using BeaconTelemetryHub.Database.Context;
using BeaconTelemetryHub.Database.DataSink;
using BeaconTelemetryHub.Database.Models;
using BeaconTelemetryHub.DataContracts.Models;
using Microsoft.EntityFrameworkCore; 
using Moq;

namespace BeaconTelemetryHub.Tests.Database
{
    [TestClass]
    public class BeaconDatabaseDataSinkTests
    {
        private DbContextOptions<BeaconDbContext> _dbOptions = null!;
        private Mock<IDbContextFactory<BeaconDbContext>> _contextFactoryMock = null!;
        private Mock<IMapper> _mapperMock = null!;

        private BeaconDatabaseDataSink CreateSut()
            => new(_contextFactoryMock.Object, _mapperMock.Object);

        [TestInitialize]
        public void Init()
        {
            _dbOptions = new DbContextOptionsBuilder<BeaconDbContext>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .Options;

            _contextFactoryMock = new Mock<IDbContextFactory<BeaconDbContext>>();
            _contextFactoryMock
                .Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new BeaconDbContext(_dbOptions));

            _mapperMock = new Mock<IMapper>();
        }

        private async Task AssertNewBeaconStoredCorrectly<TEntity>(string expectedDeviceIdentifier) where TEntity : BaseEntity
        {
            using var context = new BeaconDbContext(_dbOptions);
            var dbSet = context.Set<TEntity>();

            Assert.AreEqual(1, await dbSet.CountAsync(), $"There should be exactly one record of type {typeof(TEntity).Name}");
            Assert.AreEqual(1, await context.Beacons.CountAsync(), "Beacon count in database should be 1 (no duplicates)");

            // Using Include with a string because BaseEntity has no Beacon property.
            var entityFromDb = await dbSet
                .Include("Beacon")
                .FirstAsync();

            dynamic entity = entityFromDb;
            Assert.AreEqual(expectedDeviceIdentifier, (string)entity.Beacon.DeviceIdentifier, "DeviceIdentifier does not match.");
            Assert.AreEqual(expectedDeviceIdentifier, context.Beacons.First().DeviceIdentifier, $"The entity of type: {typeof(TEntity).Name} is not linked to the correct existing Beacon.");
        }

        [TestMethod]
        public async Task StoreBattery_WhenBeaconDoesNotExist_CreatesNewBeaconAndAddBatttery()
        {
            // Arrange
            string newDeviceIdentifier = "ABC";
            var dto = new BatteryDto(0, 0, newDeviceIdentifier, string.Empty, DateTimeOffset.Now);

            _mapperMock.Setup(m => m.Map<Beacon>(dto)).Returns(new Beacon { DeviceIdentifier = newDeviceIdentifier });
            _mapperMock.Setup(m => m.Map<Battery>(dto)).Returns(new Battery());

            var sut = CreateSut();

            // Act
            await sut.StoreBattery(dto);

            // Assert
            await AssertNewBeaconStoredCorrectly<Battery>(newDeviceIdentifier);
        }

        [TestMethod]
        public async Task StoreRssi_WhenBeaconDoesNotExist_CreatesNewBeaconAndAddRssi()
        {
            // Arrange
            string newDeviceIdentifier = "ABC";
            var dto = new RssiDto(0, newDeviceIdentifier, string.Empty, DateTimeOffset.Now);

            _mapperMock.Setup(m => m.Map<Beacon>(dto)).Returns(new Beacon { DeviceIdentifier = newDeviceIdentifier });
            _mapperMock.Setup(m => m.Map<Rssi>(dto)).Returns(new Rssi());

            var sut = CreateSut();

            // Act
            await sut.StoreRssi(dto);

            // Assert
            await AssertNewBeaconStoredCorrectly<Rssi>(newDeviceIdentifier);
        }

        [TestMethod]
        public async Task StoreTemperature_WhenBeaconDoesNotExist_CreatesNewBeaconAndAddTemperature()
        {
            // Arrange
            string newDeviceIdentifier = "ABC";
            var dto = new TemperatureDto(0, newDeviceIdentifier, string.Empty, DateTimeOffset.Now);

            _mapperMock.Setup(m => m.Map<Beacon>(dto)).Returns(new Beacon { DeviceIdentifier = newDeviceIdentifier });
            _mapperMock.Setup(m => m.Map<Temperature>(dto)).Returns(new Temperature());

            var sut = CreateSut();

            // Act
            await sut.StoreTemperature(dto);

            // Assert
            await AssertNewBeaconStoredCorrectly<Temperature>(newDeviceIdentifier);
        }

        private async Task<int> SeedNewBeacon(string deviceIdentifier)
        {
            using var seed = new BeaconDbContext(_dbOptions);
            await seed.Beacons.AddAsync(new() { DeviceIdentifier = deviceIdentifier });
            await seed.SaveChangesAsync();
            return seed.Beacons.First().Id;
        }

        private async Task AssertSameBeaconStoredCorrectly<TEntity>(string expectedDeviceIdentifier, int expectedBeaconId) where TEntity : BaseEntity
        {
            using var context = new BeaconDbContext(_dbOptions);
            var dbSet = context.Set<TEntity>();

            Assert.AreEqual(1, await dbSet.CountAsync(), $"There should be exactly one record of type {typeof(TEntity).Name}");
            Assert.AreEqual(1, await context.Beacons.CountAsync(), "Beacon count in database should be 1 (no duplicates)");

            // We use Include with a string because BaseEntity has no Beacon property.
            var entityFromDb = await dbSet
                .Include("Beacon")
                .FirstAsync();

            dynamic entity = entityFromDb;
            Assert.AreEqual(expectedDeviceIdentifier, (string)entity.Beacon.DeviceIdentifier, "DeviceIdentifier does not match.");
            Assert.AreEqual(expectedBeaconId, (int)entity.BeaconId, "The entity is not linked to the correct existing Beacon.");
        }

        [TestMethod]
        public async Task StoreBattery_WhenBeaconExists_ReusesExistingBeacon()
        {
            // Arrange
            string newDeviceIdentifier = "ABC";
            int existingBeaconId = await SeedNewBeacon(newDeviceIdentifier);

            var dto = new BatteryDto(0, 0, newDeviceIdentifier, string.Empty, DateTimeOffset.Now);
            _mapperMock.Setup(m => m.Map<Battery>(dto)).Returns(new Battery());

            var sut = CreateSut();

            // Act
            await sut.StoreBattery(dto);

            // Assert
            await AssertSameBeaconStoredCorrectly<Battery>(newDeviceIdentifier, existingBeaconId);
        }

        [TestMethod]
        public async Task StoreTemperature_WhenBeaconExists_ReusesExistingBeacon()
        {
            // Arrange
            string newDeviceIdentifier = "ABC";
            int existingBeaconId = await SeedNewBeacon(newDeviceIdentifier);

            var dto = new TemperatureDto(0, newDeviceIdentifier, string.Empty, DateTimeOffset.Now);
            _mapperMock.Setup(m => m.Map<Temperature>(dto)).Returns(new Temperature());

            var sut = CreateSut();

            // Act
            await sut.StoreTemperature(dto);

            // Assert
            await AssertSameBeaconStoredCorrectly<Temperature>(newDeviceIdentifier, existingBeaconId);
        }

        [TestMethod]
        public async Task StoreRssi_WhenBeaconExists_ReusesExistingBeacon()
        {
            // Arrange
            string newDeviceIdentifier = "ABC";
            int existingBeaconId = await SeedNewBeacon(newDeviceIdentifier);

            var dto = new RssiDto(0, newDeviceIdentifier, string.Empty, DateTimeOffset.Now);
            _mapperMock.Setup(m => m.Map<Rssi>(dto)).Returns(new Rssi());

            var sut = CreateSut();

            // Act
            await sut.StoreRssi(dto);

            // Assert
            await AssertSameBeaconStoredCorrectly<Rssi>(newDeviceIdentifier, existingBeaconId);
        }

        [TestMethod]
        public async Task StoreBattery_WhenDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => sut.StoreBattery(null!)
            );
        }

        [TestMethod]
        public async Task SaveData_WhenDuplicateDbUpdateException_DoesNotThrowException()
        {
            // Arrange
            string newDeviceIdentifier = "ABC";
            var factory = new Mock<IDbContextFactory<BeaconDbContext>>();
            factory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(() =>
                   {
                       var mock = new Mock<BeaconDbContext>(_dbOptions) { CallBase = true };
                       mock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateException("duplicate", new Exception("duplicate key")));
                       return mock.Object;
                   });

            var dto = new BatteryDto(0, 0, newDeviceIdentifier, string.Empty, DateTimeOffset.Now);
            var beacon = new Beacon { DeviceIdentifier = newDeviceIdentifier };
            var battery = new Battery();

            _mapperMock.Setup(m => m.Map<Beacon>(dto)).Returns(beacon);
            _mapperMock.Setup(m => m.Map<Battery>(dto)).Returns(battery);

            var sut = new BeaconDatabaseDataSink(factory.Object, _mapperMock.Object);

            // Act
            await sut.StoreBattery(dto);
            // Test should end without any exception
        }

        [TestMethod]
        public async Task SaveData_WhenUnexpectedException_ThrowsInvalidOperationException()
        {
            // Arrange
            var factory = new Mock<IDbContextFactory<BeaconDbContext>>();
            factory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                   .ThrowsAsync(new Exception());

            var sut = new BeaconDatabaseDataSink(factory.Object, _mapperMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => sut.StoreBattery(new BatteryDto (0, 0, "ABC", string.Empty, DateTimeOffset.Now))
            );
        }
    }
}
