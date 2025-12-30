using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PemitManagement.Migrations
{
    /// <inheritdoc />
    public partial class FixSnakeCaseColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "permits",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "ClosedBy",
                table: "permits",
                newName: "closed_by");

            migrationBuilder.RenameColumn(
                name: "ClosedAt",
                table: "permits",
                newName: "closed_at");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "permit_violations",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "ClosedBy",
                table: "permit_violations",
                newName: "closed_by");

            migrationBuilder.RenameColumn(
                name: "ClosedAt",
                table: "permit_violations",
                newName: "closed_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "permits",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "closed_by",
                table: "permits",
                newName: "ClosedBy");

            migrationBuilder.RenameColumn(
                name: "closed_at",
                table: "permits",
                newName: "ClosedAt");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "permit_violations",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "closed_by",
                table: "permit_violations",
                newName: "ClosedBy");

            migrationBuilder.RenameColumn(
                name: "closed_at",
                table: "permit_violations",
                newName: "ClosedAt");
        }
    }
}
