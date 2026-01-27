using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaconTelemetryHub.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedRssi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rssi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Primary Key")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BeaconId = table.Column<int>(type: "int", nullable: false, comment: "Beacon foreign key"),
                    SignalStrengthdBm = table.Column<short>(type: "smallint", nullable: false, comment: "Rssi value in dBm"),
                    FoundTimestamp = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Data found timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rssi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rssi_Beacons_BeaconId",
                        column: x => x.BeaconId,
                        principalTable: "Beacons",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Rssi_BeaconId",
                table: "Rssi",
                column: "BeaconId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rssi");
        }
    }
}
