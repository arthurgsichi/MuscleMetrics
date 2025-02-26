using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuscleMetrics.Migrations
{
    /// <inheritdoc />
    public partial class DietaAtualizada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Altura",
                table: "Dieta",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BF",
                table: "Dieta",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Peso",
                table: "Dieta",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Altura",
                table: "Dieta");

            migrationBuilder.DropColumn(
                name: "BF",
                table: "Dieta");

            migrationBuilder.DropColumn(
                name: "Peso",
                table: "Dieta");
        }
    }
}
