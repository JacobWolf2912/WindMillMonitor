using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WindTurbineMonitor.Api.Migrations
{
    /// <inheritdoc />
    public partial class PendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Username", "PasswordHash", "CreatedAt" },
                values: new object[] { new Guid("550e8400-e29b-41d4-a716-446655440000"), "testuser", "$2a$12$R9h7cIPz0gi.URNNGHQ1Kuo3VszQiLRVMwCS7tHsFJd6PZX5BWMhe", DateTime.UtcNow }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000")
            );
        }
    }
}
