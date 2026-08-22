using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToMainApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedDescriptionToVehicleCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "VehicleCategories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "VehicleCategories");
        }
    }
}
