using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class SearchIndicesBrand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_brands_TenantId_Name",
                schema: "affiliate-service",
                table: "brands",
                columns: new[] { "TenantId", "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_brands_TenantId_Name",
                schema: "affiliate-service",
                table: "brands");
        }
    }
}
