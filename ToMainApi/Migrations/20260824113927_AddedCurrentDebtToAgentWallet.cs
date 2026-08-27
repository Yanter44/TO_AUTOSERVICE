using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToMainApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedCurrentDebtToAgentWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentDebt",
                table: "Wallets",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentDebt",
                table: "Wallets");
        }
    }
}
