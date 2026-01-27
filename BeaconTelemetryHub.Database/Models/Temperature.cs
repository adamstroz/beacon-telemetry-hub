using Microsoft.EntityFrameworkCore;

namespace BeaconTelemetryHub.Database.Models
{
    [Index(nameof(FoundTimestamp), IsUnique = true)]
    internal class Temperature : BaseEntity
    {
        [Comment("Temperature report by beacon")]
        public double TemperatureCelsius { get; set; }
        [Comment("Beacon foreign key")]
        public int BeaconId { get; set; } 
        public Beacon Beacon { get; set; } = null!;
        [Comment("Data found timestamp")]
        public DateTimeOffset FoundTimestamp { get; set; } = DateTimeOffset.Now;
    }
}
