using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WindTurbineMonitor.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedTestUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys
            migrationBuilder.DropForeignKey(name: "FK_Alerts_Turbines_TurbineId", table: "Alerts");
            migrationBuilder.DropForeignKey(name: "FK_CommandLogs_Turbines_TurbineId", table: "CommandLogs");
            migrationBuilder.DropForeignKey(name: "FK_TurbineMetrics_Turbines_TurbineId", table: "TurbineMetrics");

            // Drop indexes
            migrationBuilder.DropIndex(name: "IX_TurbineMetrics_TurbineId_Timestamp", table: "TurbineMetrics");
            migrationBuilder.DropIndex(name: "IX_Alerts_TurbineId_Timestamp", table: "Alerts");
            migrationBuilder.DropIndex(name: "IX_CommandLogs_TurbineId_IssuedAt", table: "CommandLogs");

            // Use raw SQL to drop and recreate Turbines.Id column
            migrationBuilder.Sql("ALTER TABLE Turbines DROP CONSTRAINT PK_Turbines;");
            migrationBuilder.Sql("ALTER TABLE Turbines DROP COLUMN Id;");
            migrationBuilder.Sql("ALTER TABLE Turbines ADD Id NVARCHAR(450) NOT NULL DEFAULT N'';");
            migrationBuilder.Sql("ALTER TABLE Turbines ADD CONSTRAINT PK_Turbines PRIMARY KEY (Id);");

            // Drop and recreate TurbineMetrics.TurbineId
            migrationBuilder.Sql("ALTER TABLE TurbineMetrics DROP COLUMN TurbineId;");
            migrationBuilder.Sql("ALTER TABLE TurbineMetrics ADD TurbineId NVARCHAR(450) NOT NULL DEFAULT N'';");

            // Drop and recreate CommandLogs.TurbineId
            migrationBuilder.Sql("ALTER TABLE CommandLogs DROP COLUMN TurbineId;");
            migrationBuilder.Sql("ALTER TABLE CommandLogs ADD TurbineId NVARCHAR(450) NOT NULL DEFAULT N'';");

            // Drop and recreate Alerts.TurbineId
            migrationBuilder.Sql("ALTER TABLE Alerts DROP COLUMN TurbineId;");
            migrationBuilder.Sql("ALTER TABLE Alerts ADD TurbineId NVARCHAR(450) NOT NULL DEFAULT N'';");

            // Recreate foreign keys
            migrationBuilder.AddForeignKey(name: "FK_Alerts_Turbines_TurbineId", table: "Alerts", column: "TurbineId", principalTable: "Turbines", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_CommandLogs_Turbines_TurbineId", table: "CommandLogs", column: "TurbineId", principalTable: "Turbines", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_TurbineMetrics_Turbines_TurbineId", table: "TurbineMetrics", column: "TurbineId", principalTable: "Turbines", principalColumn: "Id", onDelete: ReferentialAction.Cascade);

            // Recreate indexes
            migrationBuilder.CreateIndex(name: "IX_TurbineMetrics_TurbineId_Timestamp", table: "TurbineMetrics", columns: new[] { "TurbineId", "Timestamp" });
            migrationBuilder.CreateIndex(name: "IX_Alerts_TurbineId_Timestamp", table: "Alerts", columns: new[] { "TurbineId", "Timestamp" });
            migrationBuilder.CreateIndex(name: "IX_CommandLogs_TurbineId_IssuedAt", table: "CommandLogs", columns: new[] { "TurbineId", "IssuedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys
            migrationBuilder.DropForeignKey(name: "FK_Alerts_Turbines_TurbineId", table: "Alerts");
            migrationBuilder.DropForeignKey(name: "FK_CommandLogs_Turbines_TurbineId", table: "CommandLogs");
            migrationBuilder.DropForeignKey(name: "FK_TurbineMetrics_Turbines_TurbineId", table: "TurbineMetrics");

            // Drop indexes
            migrationBuilder.DropIndex(name: "IX_TurbineMetrics_TurbineId_Timestamp", table: "TurbineMetrics");
            migrationBuilder.DropIndex(name: "IX_Alerts_TurbineId_Timestamp", table: "Alerts");
            migrationBuilder.DropIndex(name: "IX_CommandLogs_TurbineId_IssuedAt", table: "CommandLogs");

            // Recreate as INT with IDENTITY
            migrationBuilder.Sql("ALTER TABLE Turbines DROP CONSTRAINT PK_Turbines;");
            migrationBuilder.Sql("ALTER TABLE Turbines DROP COLUMN Id;");
            migrationBuilder.Sql("ALTER TABLE Turbines ADD Id INT NOT NULL IDENTITY(1,1);");
            migrationBuilder.Sql("ALTER TABLE Turbines ADD CONSTRAINT PK_Turbines PRIMARY KEY (Id);");

            migrationBuilder.Sql("ALTER TABLE TurbineMetrics DROP COLUMN TurbineId;");
            migrationBuilder.Sql("ALTER TABLE TurbineMetrics ADD TurbineId INT NOT NULL DEFAULT 0;");

            migrationBuilder.Sql("ALTER TABLE CommandLogs DROP COLUMN TurbineId;");
            migrationBuilder.Sql("ALTER TABLE CommandLogs ADD TurbineId INT NOT NULL DEFAULT 0;");

            migrationBuilder.Sql("ALTER TABLE Alerts DROP COLUMN TurbineId;");
            migrationBuilder.Sql("ALTER TABLE Alerts ADD TurbineId INT NOT NULL DEFAULT 0;");

            // Recreate foreign keys
            migrationBuilder.AddForeignKey(name: "FK_Alerts_Turbines_TurbineId", table: "Alerts", column: "TurbineId", principalTable: "Turbines", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_CommandLogs_Turbines_TurbineId", table: "CommandLogs", column: "TurbineId", principalTable: "Turbines", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_TurbineMetrics_Turbines_TurbineId", table: "TurbineMetrics", column: "TurbineId", principalTable: "Turbines", principalColumn: "Id", onDelete: ReferentialAction.Cascade);

            // Recreate indexes
            migrationBuilder.CreateIndex(name: "IX_TurbineMetrics_TurbineId_Timestamp", table: "TurbineMetrics", columns: new[] { "TurbineId", "Timestamp" });
            migrationBuilder.CreateIndex(name: "IX_Alerts_TurbineId_Timestamp", table: "Alerts", columns: new[] { "TurbineId", "Timestamp" });
            migrationBuilder.CreateIndex(name: "IX_CommandLogs_TurbineId_IssuedAt", table: "CommandLogs", columns: new[] { "TurbineId", "IssuedAt" });
        }
    }
}
