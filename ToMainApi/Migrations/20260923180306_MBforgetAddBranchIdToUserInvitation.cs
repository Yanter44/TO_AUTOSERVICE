using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToMainApi.Migrations
{
    /// <inheritdoc />
    public partial class MBforgetAddBranchIdToUserInvitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "UserInvitations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_BranchId",
                table: "UserInvitations",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInvitations_AgentBranches_BranchId",
                table: "UserInvitations",
                column: "BranchId",
                principalTable: "AgentBranches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInvitations_AgentBranches_BranchId",
                table: "UserInvitations");

            migrationBuilder.DropIndex(
                name: "IX_UserInvitations_BranchId",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "UserInvitations");
        }
    }
}
