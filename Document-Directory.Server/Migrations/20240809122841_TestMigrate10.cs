using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document_Directory.Server.Migrations
{
    /// <inheritdoc />
    public partial class TestMigrate10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeHierarchy_Nodes_FoldersId",
                table: "NodeHierarchy");

            migrationBuilder.DropForeignKey(
                name: "FK_NodeHierarchy_Nodes_NodesId",
                table: "NodeHierarchy");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Groups_GroupsId",
                table: "UserGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Users_UsersId",
                table: "UserGroups");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "UserGroups",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "GroupsId",
                table: "UserGroups",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_UsersId",
                table: "UserGroups",
                newName: "IX_UserGroups_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_GroupsId",
                table: "UserGroups",
                newName: "IX_UserGroups_GroupId");

            migrationBuilder.RenameColumn(
                name: "NodesId",
                table: "NodeHierarchy",
                newName: "NodeId");

            migrationBuilder.RenameColumn(
                name: "FoldersId",
                table: "NodeHierarchy",
                newName: "FolderId");

            migrationBuilder.RenameIndex(
                name: "IX_NodeHierarchy_NodesId",
                table: "NodeHierarchy",
                newName: "IX_NodeHierarchy_NodeId");

            migrationBuilder.RenameIndex(
                name: "IX_NodeHierarchy_FoldersId",
                table: "NodeHierarchy",
                newName: "IX_NodeHierarchy_FolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeHierarchy_Nodes_FolderId",
                table: "NodeHierarchy",
                column: "FolderId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NodeHierarchy_Nodes_NodeId",
                table: "NodeHierarchy",
                column: "NodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Groups_GroupId",
                table: "UserGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Users_UserId",
                table: "UserGroups",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NodeHierarchy_Nodes_FolderId",
                table: "NodeHierarchy");

            migrationBuilder.DropForeignKey(
                name: "FK_NodeHierarchy_Nodes_NodeId",
                table: "NodeHierarchy");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Groups_GroupId",
                table: "UserGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_Users_UserId",
                table: "UserGroups");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserGroups",
                newName: "UsersId");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "UserGroups",
                newName: "GroupsId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_UserId",
                table: "UserGroups",
                newName: "IX_UserGroups_UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroups",
                newName: "IX_UserGroups_GroupsId");

            migrationBuilder.RenameColumn(
                name: "NodeId",
                table: "NodeHierarchy",
                newName: "NodesId");

            migrationBuilder.RenameColumn(
                name: "FolderId",
                table: "NodeHierarchy",
                newName: "FoldersId");

            migrationBuilder.RenameIndex(
                name: "IX_NodeHierarchy_NodeId",
                table: "NodeHierarchy",
                newName: "IX_NodeHierarchy_NodesId");

            migrationBuilder.RenameIndex(
                name: "IX_NodeHierarchy_FolderId",
                table: "NodeHierarchy",
                newName: "IX_NodeHierarchy_FoldersId");

            migrationBuilder.AddForeignKey(
                name: "FK_NodeHierarchy_Nodes_FoldersId",
                table: "NodeHierarchy",
                column: "FoldersId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NodeHierarchy_Nodes_NodesId",
                table: "NodeHierarchy",
                column: "NodesId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Groups_GroupsId",
                table: "UserGroups",
                column: "GroupsId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_Users_UsersId",
                table: "UserGroups",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
