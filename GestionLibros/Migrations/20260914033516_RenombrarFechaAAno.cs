using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionLibros.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarFechaAAno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaPublicacion",
                table: "Libros");

            migrationBuilder.AddColumn<int>(
                name: "AnoPublicacion",
                table: "Libros",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnoPublicacion",
                table: "Libros");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPublicacion",
                table: "Libros",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
