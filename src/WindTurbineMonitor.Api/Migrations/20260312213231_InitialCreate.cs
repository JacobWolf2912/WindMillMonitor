using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WindTurbineMonitor.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Turbines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MqttTopicPrefix = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InstalledAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turbines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TurbineId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Turbines_TurbineId",
                        column: x => x.TurbineId,
                        principalTable: "Turbines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommandLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TurbineId = table.Column<int>(type: "int", nullable: false),
                    IssuedByUsername = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CommandType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParametersJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommandLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommandLogs_Turbines_TurbineId",
                        column: x => x.TurbineId,
                        principalTable: "Turbines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TurbineMetrics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TurbineId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    RotorRpm = table.Column<double>(type: "float", nullable: true),
                    PowerOutputKw = table.Column<double>(type: "float", nullable: true),
                    WindSpeedMs = table.Column<double>(type: "float", nullable: true),
                    WindDirectionDeg = table.Column<double>(type: "float", nullable: true),
                    NacelleTemperatureCelsius = table.Column<double>(type: "float", nullable: true),
                    GearboxTemperatureCelsius = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurbineMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurbineMetrics_Turbines_TurbineId",
                        column: x => x.TurbineId,
                        principalTable: "Turbines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Timestamp",
                table: "Alerts",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_TurbineId_Timestamp",
                table: "Alerts",
                columns: new[] { "TurbineId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_CommandLogs_IssuedAt",
                table: "CommandLogs",
                column: "IssuedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CommandLogs_TurbineId_IssuedAt",
                table: "CommandLogs",
                columns: new[] { "TurbineId", "IssuedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TurbineMetrics_Timestamp",
                table: "TurbineMetrics",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_TurbineMetrics_TurbineId_Timestamp",
                table: "TurbineMetrics",
                columns: new[] { "TurbineId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "CommandLogs");

            migrationBuilder.DropTable(
                name: "TurbineMetrics");

            migrationBuilder.DropTable(
                name: "Turbines");
        }
    }
}
