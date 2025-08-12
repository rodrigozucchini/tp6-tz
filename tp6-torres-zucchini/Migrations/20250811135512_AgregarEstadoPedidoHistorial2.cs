using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tp6_torres_zucchini.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEstadoPedidoHistorial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PedidoHistorial",
                table: "PedidoHistorial");

            migrationBuilder.RenameTable(
                name: "PedidoHistorial",
                newName: "PedidoHistoriales");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PedidoHistoriales",
                table: "PedidoHistoriales",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PedidoHistoriales",
                table: "PedidoHistoriales");

            migrationBuilder.RenameTable(
                name: "PedidoHistoriales",
                newName: "PedidoHistorial");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PedidoHistorial",
                table: "PedidoHistorial",
                column: "Id");
        }
    }
}
