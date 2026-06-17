using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WindTurbineMonitor.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTelemetryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NacelleTemperatureCelsius",
                table: "TurbineMetrics",
                newName: "VibrationMs2");

            migrationBuilder.AddColumn<double>(
                name: "AmbientTemperatureCelsius",
                table: "TurbineMetrics",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BladePitchDeg",
                table: "TurbineMetrics",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GeneratorTemperatureCelsius",
                table: "TurbineMetrics",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NacelleDirectionDeg",
                table: "TurbineMetrics",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmbientTemperatureCelsius",
                table: "TurbineMetrics");

            migrationBuilder.DropColumn(
                name: "BladePitchDeg",
                table: "TurbineMetrics");

            migrationBuilder.DropColumn(
                name: "GeneratorTemperatureCelsius",
                table: "TurbineMetrics");

            migrationBuilder.DropColumn(
                name: "NacelleDirectionDeg",
                table: "TurbineMetrics");

            migrationBuilder.RenameColumn(
                name: "VibrationMs2",
                table: "TurbineMetrics",
                newName: "NacelleTemperatureCelsius");
        }
    }
}
