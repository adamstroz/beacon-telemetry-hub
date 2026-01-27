using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaconTelemetryHub.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Temperature_FoundTimestamp",
                table: "Temperature",
                column: "FoundTimestamp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rssi_FoundTimestamp",
                table: "Rssi",
                column: "FoundTimestamp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Battery_FoundTimestamp",
                table: "Battery",
                column: "FoundTimestamp",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Temperature_FoundTimestamp",
                table: "Temperature");

            migrationBuilder.DropIndex(
                name: "IX_Rssi_FoundTimestamp",
                table: "Rssi");

            migrationBuilder.DropIndex(
                name: "IX_Battery_FoundTimestamp",
                table: "Battery");
        }
    }
}
