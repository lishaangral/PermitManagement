using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PemitManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddActiveToUserPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "active",
                table: "user_permissions",
                type: "tinyint(1)",
                nullable: true,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "active",
                table: "user_permissions");
        }
    }
}
