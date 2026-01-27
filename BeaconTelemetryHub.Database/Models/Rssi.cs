using Microsoft.EntityFrameworkCore;

namespace BeaconTelemetryHub.Database.Models
{
    [Index(nameof(FoundTimestamp), IsUnique = true)]
    internal class Rssi : BaseEntity
    {
        [Comment("Beacon foreign key")]
        public int BeaconId { get; set; }
        public Beacon Beacon { get; set; } = null!;
        [Comment("Rssi value in dBm")]
        public short SignalStrengthdBm { get; set; }
        [Comment("Data found timestamp")]
        public DateTimeOffset FoundTimestamp { get; set; } = DateTimeOffset.Now;
    }
}
