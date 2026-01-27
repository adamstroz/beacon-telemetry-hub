using Microsoft.EntityFrameworkCore;

namespace BeaconTelemetryHub.Database.Models
{
    internal abstract class BaseEntity
    {
        [Comment("Primary Key")]
        public int Id { get; set; }
    }

}
