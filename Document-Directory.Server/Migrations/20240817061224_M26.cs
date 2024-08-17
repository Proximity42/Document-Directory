using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document_Directory.Server.Migrations
{
    /// <inheritdoc />
    public partial class M26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Users_UserId",
                table: "Nodes");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Nodes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Users_UserId",
                table: "Nodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Users_UserId",
                table: "Nodes");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Nodes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Users_UserId",
                table: "Nodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
