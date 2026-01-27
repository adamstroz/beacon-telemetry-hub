using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaconTelemetryHub.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Beacons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Primary Key")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DeviceIdentifier = table.Column<string>(type: "varchar(255)", nullable: false, comment: "Beacon manufacturer UUID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceBleAddress = table.Column<string>(type: "longtext", nullable: false, comment: "Beacon Ble address")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beacons", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Battery",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Primary Key")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BatteryLevelPercent = table.Column<int>(type: "int", nullable: false, comment: "Battery level in %"),
                    BatteryVoltageMillivolts = table.Column<int>(type: "int", nullable: false, comment: "Battery level in millivolts"),
                    BeaconId = table.Column<int>(type: "int", nullable: false, comment: "Beacon foreign key"),
                    FoundTimestamp = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Data found timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Battery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Battery_Beacons_BeaconId",
                        column: x => x.BeaconId,
                        principalTable: "Beacons",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Temperature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Primary Key")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TemperatureCelsius = table.Column<double>(type: "double", nullable: false, comment: "Temperature report by beacon"),
                    BeaconId = table.Column<int>(type: "int", nullable: false, comment: "Beacon foreign key"),
                    FoundTimestamp = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false, comment: "Data found timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temperature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Temperature_Beacons_BeaconId",
                        column: x => x.BeaconId,
                        principalTable: "Beacons",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Battery_BeaconId",
                table: "Battery",
                column: "BeaconId");

            migrationBuilder.CreateIndex(
                name: "IX_Beacons_DeviceIdentifier",
                table: "Beacons",
                column: "DeviceIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Temperature_BeaconId",
                table: "Temperature",
                column: "BeaconId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Battery");

            migrationBuilder.DropTable(
                name: "Temperature");

            migrationBuilder.DropTable(
                name: "Beacons");
        }
    }
}
