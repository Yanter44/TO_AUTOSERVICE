using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToMainApi.Migrations
{
    /// <inheritdoc />
    public partial class RemasteredLongtitudeAndLatitudeInPtoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                                    ALTER TABLE ""Ptos""
                                    ALTER COLUMN ""Latitude"" TYPE double precision
                                    USING ""Latitude""::double precision;
                                ");

            migrationBuilder.Sql(@"
                                    ALTER TABLE ""Ptos""
                                    ALTER COLUMN ""Longitude"" TYPE double precision
                                    USING ""Longitude""::double precision;
                                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                                ALTER TABLE ""Ptos""
                                ALTER COLUMN ""Latitude"" TYPE character varying(50)
                                USING ""Latitude""::text;
                            ");

            migrationBuilder.Sql(@"
                                ALTER TABLE ""Ptos""
                                ALTER COLUMN ""Longitude"" TYPE character varying(50)
                                USING ""Longitude""::text;
                            ");
        }
    }
}
