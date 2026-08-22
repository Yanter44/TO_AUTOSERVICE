using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToMainApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedRejectReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgentId",
                table: "Applications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "Applications",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_AgentId",
                table: "Applications",
                column: "AgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_AgentProfiles_AgentId",
                table: "Applications",
                column: "AgentId",
                principalTable: "AgentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_AgentProfiles_AgentId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_AgentId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "Applications");
        }
    }
}
