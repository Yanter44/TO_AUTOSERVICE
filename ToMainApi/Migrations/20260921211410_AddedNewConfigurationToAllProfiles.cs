using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToMainApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewConfigurationToAllProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentProfiles_Users_UserId",
                table: "AgentProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentProfiles_Users_UserId1",
                table: "AgentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_AgentProfiles_UserId1",
                table: "AgentProfiles");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "AgentProfiles");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentProfiles_Users_UserId",
                table: "AgentProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentProfiles_Users_UserId",
                table: "AgentProfiles");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "AgentProfiles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgentProfiles_UserId1",
                table: "AgentProfiles",
                column: "UserId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentProfiles_Users_UserId",
                table: "AgentProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentProfiles_Users_UserId1",
                table: "AgentProfiles",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
