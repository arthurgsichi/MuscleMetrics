using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuscleMetrics.Migrations
{
    /// <inheritdoc />
    public partial class UsuarioAtualizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FatorAtividade",
                table: "Usuario",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "GastoCalórico",
                table: "Usuario",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FatorAtividade",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "GastoCalórico",
                table: "Usuario");
        }
    }
}
