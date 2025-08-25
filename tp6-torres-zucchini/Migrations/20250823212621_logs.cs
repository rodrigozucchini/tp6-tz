using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tp6_torres_zucchini.Migrations
{
    /// <inheritdoc />
    public partial class logs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LgoPeticiones",
                table: "LgoPeticiones");

            migrationBuilder.RenameTable(
                name: "LgoPeticiones",
                newName: "LogPeticion");

            migrationBuilder.AlterColumn<string>(
                name: "RespuestaComando",
                table: "LogPeticion",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "ConexionId",
                table: "LogPeticion",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Comando",
                table: "LogPeticion",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogPeticion",
                table: "LogPeticion",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LogPeticion",
                table: "LogPeticion");

            migrationBuilder.RenameTable(
                name: "LogPeticion",
                newName: "LgoPeticiones");

            migrationBuilder.AlterColumn<string>(
                name: "RespuestaComando",
                table: "LgoPeticiones",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "ConexionId",
                table: "LgoPeticiones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comando",
                table: "LgoPeticiones",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LgoPeticiones",
                table: "LgoPeticiones",
                column: "Id");
        }
    }
}
