using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PemitManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddPermitAndObservationLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "permits",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "permits",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "permits",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "permit_violations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "permit_violations",
                type: "longtext",
                nullable: true,
                collation: "utf8mb4_general_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "permit_violations",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "permits");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "permits");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "permits");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "permit_violations");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "permit_violations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "permit_violations");
        }
    }
}
