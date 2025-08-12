using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tp6_torres_zucchini.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEstadoPedidoHistorial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PedidoHistoriales",
                table: "PedidoHistoriales");

            migrationBuilder.RenameTable(
                name: "PedidoHistoriales",
                newName: "PedidoHistorial");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "PedidoHistorial",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PedidoHistorial",
                table: "PedidoHistorial",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PedidoHistorial",
                table: "PedidoHistorial");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "PedidoHistorial");

            migrationBuilder.RenameTable(
                name: "PedidoHistorial",
                newName: "PedidoHistoriales");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PedidoHistoriales",
                table: "PedidoHistoriales",
                column: "Id");
        }
    }
}
