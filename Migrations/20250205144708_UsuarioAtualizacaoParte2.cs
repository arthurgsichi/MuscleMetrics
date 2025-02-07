using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuscleMetrics.Migrations
{
    /// <inheritdoc />
    public partial class UsuarioAtualizacaoParte2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CaloriaDieta",
                table: "Usuario",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Carboidrato",
                table: "Usuario",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Gordura",
                table: "Usuario",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Proteina",
                table: "Usuario",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaloriaDieta",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Carboidrato",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Gordura",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Proteina",
                table: "Usuario");
        }
    }
}
