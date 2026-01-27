using Microsoft.EntityFrameworkCore;

namespace BeaconTelemetryHub.Database.Models
{
    [Index(nameof(FoundTimestamp), IsUnique = true)]
    internal class Battery : BaseEntity
    {
        [Comment("Battery level in %")]
        public double BatteryLevelPercent { get; set; }

        [Comment("Battery level in millivolts")]
        public double BatteryVoltageMillivolts { get; set; }
        [Comment("Beacon foreign key")]
        public int BeaconId { get; set; }
        public Beacon Beacon { get; set; } = null!;

        [Comment("Data found timestamp")]
        public DateTimeOffset FoundTimestamp { get; set; } = DateTimeOffset.Now;
    }

}
