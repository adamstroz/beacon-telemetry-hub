using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaconTelemetryHub.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangedBatteryUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "BatteryVoltageMillivolts",
                table: "Battery",
                type: "double",
                nullable: false,
                comment: "Battery level in millivolts",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Battery level in millivolts");

            migrationBuilder.AlterColumn<double>(
                name: "BatteryLevelPercent",
                table: "Battery",
                type: "double",
                nullable: false,
                comment: "Battery level in %",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Battery level in %");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BatteryVoltageMillivolts",
                table: "Battery",
                type: "int",
                nullable: false,
                comment: "Battery level in millivolts",
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "Battery level in millivolts");

            migrationBuilder.AlterColumn<int>(
                name: "BatteryLevelPercent",
                table: "Battery",
                type: "int",
                nullable: false,
                comment: "Battery level in %",
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "Battery level in %");
        }
    }
}
