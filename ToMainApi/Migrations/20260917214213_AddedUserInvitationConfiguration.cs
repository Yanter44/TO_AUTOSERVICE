using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToMainApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserInvitationConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInvitations_Users_CreatedById",
                table: "UserInvitations");

            migrationBuilder.DropIndex(
                name: "IX_UserInvitations_CreatedById",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "UserInvitations");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "UserInvitations",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "RoleType",
                table: "UserInvitations",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsed",
                table: "UserInvitations",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserInvitations",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_CreatedByUserId",
                table: "UserInvitations",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_Email",
                table: "UserInvitations",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_Token",
                table: "UserInvitations",
                column: "Token",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserInvitations_Users_CreatedByUserId",
                table: "UserInvitations",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInvitations_Users_CreatedByUserId",
                table: "UserInvitations");

            migrationBuilder.DropIndex(
                name: "IX_UserInvitations_CreatedByUserId",
                table: "UserInvitations");

            migrationBuilder.DropIndex(
                name: "IX_UserInvitations_Email",
                table: "UserInvitations");

            migrationBuilder.DropIndex(
                name: "IX_UserInvitations_Token",
                table: "UserInvitations");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "UserInvitations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "RoleType",
                table: "UserInvitations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<bool>(
                name: "IsUsed",
                table: "UserInvitations",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserInvitations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "UserInvitations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_CreatedById",
                table: "UserInvitations",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInvitations_Users_CreatedById",
                table: "UserInvitations",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
