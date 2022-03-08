using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class Country_ISOCode_to_Numeric_coulumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ISOCode",
                schema: "affiliate-service",
                table: "countries",
                newName: "Numeric");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Numeric",
                schema: "affiliate-service",
                table: "countries",
                newName: "ISOCode");
        }
    }
}
