using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToMainApi.Migrations
{
    /// <inheritdoc />
    public partial class ReworkedSomeConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentBranches_AgentProfiles_OwnerAgentId",
                table: "AgentBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentProfiles_AgentBranches_BranchId",
                table: "AgentProfiles");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentBranches_AgentProfiles_OwnerAgentId",
                table: "AgentBranches",
                column: "OwnerAgentId",
                principalTable: "AgentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentProfiles_AgentBranches_BranchId",
                table: "AgentProfiles",
                column: "BranchId",
                principalTable: "AgentBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentBranches_AgentProfiles_OwnerAgentId",
                table: "AgentBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentProfiles_AgentBranches_BranchId",
                table: "AgentProfiles");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentBranches_AgentProfiles_OwnerAgentId",
                table: "AgentBranches",
                column: "OwnerAgentId",
                principalTable: "AgentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentProfiles_AgentBranches_BranchId",
                table: "AgentProfiles",
                column: "BranchId",
                principalTable: "AgentBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
