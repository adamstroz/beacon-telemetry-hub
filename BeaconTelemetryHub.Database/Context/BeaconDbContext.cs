using BeaconTelemetryHub.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace BeaconTelemetryHub.Database.Context
{
    internal class BeaconDbContext(DbContextOptions<BeaconDbContext> options) : DbContext(options)
    {
        /// <summary>
        /// Gets or sets the collection of beacon battery records.
        /// </summary>
        public DbSet<Battery> Battery { get; set; }

        /// <summary>
        /// Gets or sets the collection of beacon temperature data.
        /// </summary>
        public DbSet<Temperature> Temperature { get; set; }

        /// <summary>
        /// Gets or sets the collection of found beacon devices.
        /// </summary>
        public DbSet<Beacon> Beacons { get; set; }

        /// <summary>
        /// Gets or sets the collection of beacon RSSI (Signal Strength) data.
        /// </summary>
        public DbSet<Rssi> Rssi { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Battery>()
                        .HasOne(b => b.Beacon)
                        .WithMany()
                        .HasForeignKey(b => b.BeaconId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Temperature>()
                        .HasOne(b => b.Beacon)
                        .WithMany()
                        .HasForeignKey(b => b.BeaconId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Rssi>()
                        .HasOne(b => b.Beacon)
                        .WithMany()
                        .HasForeignKey(b => b.BeaconId)
                        .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
