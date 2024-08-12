using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document_Directory.Server.Migrations
{
    /// <inheritdoc />
    public partial class TestMigrate11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeAccess_Groups_GroupId",
                table: "NodeAccess");

            migrationBuilder.DropForeignKey(
                name: "FK_NodeAccess_Users_UserId",
                table: "NodeAccess");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "NodeAccess",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "NodeAccess",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NodeAccess_Groups_GroupId",
                table: "NodeAccess",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NodeAccess_Users_UserId",
                table: "NodeAccess",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeAccess_Groups_GroupId",
                table: "NodeAccess");

            migrationBuilder.DropForeignKey(
                name: "FK_NodeAccess_Users_UserId",
                table: "NodeAccess");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "NodeAccess",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "NodeAccess",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeAccess_Groups_GroupId",
                table: "NodeAccess",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeAccess_Users_UserId",
                table: "NodeAccess",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
