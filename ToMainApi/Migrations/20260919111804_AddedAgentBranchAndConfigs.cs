using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ToMainApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedAgentBranchAndConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentProfiles_Users_UserId",
                table: "AgentProfiles");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "AgentProfiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentAgentId",
                table: "AgentProfiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "AgentProfiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "AgentProfiles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WalletId",
                table: "AgentProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AgentBranches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Fee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    OwnerAgentId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentBranches_AgentProfiles_OwnerAgentId",
                        column: x => x.OwnerAgentId,
                        principalTable: "AgentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgentProfiles_BranchId",
                table: "AgentProfiles",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentProfiles_ParentAgentId",
                table: "AgentProfiles",
                column: "ParentAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentProfiles_Path",
                table: "AgentProfiles",
                column: "Path");

            migrationBuilder.CreateIndex(
                name: "IX_AgentProfiles_UserId1",
                table: "AgentProfiles",
                column: "UserId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgentBranches_OwnerAgentId",
                table: "AgentBranches",
                column: "OwnerAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentProfiles_AgentBranches_BranchId",
                table: "AgentProfiles",
                column: "BranchId",
                principalTable: "AgentBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentProfiles_AgentProfiles_ParentAgentId",
                table: "AgentProfiles",
                column: "ParentAgentId",
                principalTable: "AgentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AgentProfiles_AgentBranches_BranchId",
                table: "AgentProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentProfiles_AgentProfiles_ParentAgentId",
                table: "AgentProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentProfiles_Users_UserId",
                table: "AgentProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentProfiles_Users_UserId1",
                table: "AgentProfiles");

            migrationBuilder.DropTable(
                name: "AgentBranches");

            migrationBuilder.DropIndex(
                name: "IX_AgentProfiles_BranchId",
                table: "AgentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_AgentProfiles_ParentAgentId",
                table: "AgentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_AgentProfiles_Path",
                table: "AgentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_AgentProfiles_UserId1",
                table: "AgentProfiles");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "AgentProfiles");

            migrationBuilder.DropColumn(
                name: "ParentAgentId",
                table: "AgentProfiles");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "AgentProfiles");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "AgentProfiles");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "AgentProfiles");

            migrationBuilder.AddForeignKey(
                name: "FK_AgentProfiles_Users_UserId",
                table: "AgentProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
