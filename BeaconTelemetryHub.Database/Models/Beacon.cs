using Microsoft.EntityFrameworkCore;

namespace BeaconTelemetryHub.Database.Models
{
    [Index(nameof(DeviceIdentifier), IsUnique = true)]
    internal class Beacon : BaseEntity
    {
        [Comment("Beacon manufacturer UUID")]
        public string DeviceIdentifier { get; set; } = string.Empty;
        [Comment("Beacon Ble address")]
        public string DeviceBleAddress { get; set; } = string.Empty;
    }
}
