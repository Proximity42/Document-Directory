using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document_Directory.Server.Migrations
{
    /// <inheritdoc />
    public partial class TestMigrate13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeHierarchy_Nodes_Folderid",
                table: "NodeHierarchy");

            migrationBuilder.RenameColumn(
                name: "Folderid",
                table: "NodeHierarchy",
                newName: "FolderId");

            migrationBuilder.RenameIndex(
                name: "IX_NodeHierarchy_Folderid",
                table: "NodeHierarchy",
                newName: "IX_NodeHierarchy_FolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeHierarchy_Nodes_FolderId",
                table: "NodeHierarchy",
                column: "FolderId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeHierarchy_Nodes_FolderId",
                table: "NodeHierarchy");

            migrationBuilder.RenameColumn(
                name: "FolderId",
                table: "NodeHierarchy",
                newName: "Folderid");

            migrationBuilder.RenameIndex(
                name: "IX_NodeHierarchy_FolderId",
                table: "NodeHierarchy",
                newName: "IX_NodeHierarchy_Folderid");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeHierarchy_Nodes_Folderid",
                table: "NodeHierarchy",
                column: "Folderid",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
