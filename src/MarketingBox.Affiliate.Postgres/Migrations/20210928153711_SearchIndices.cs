using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class SearchIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_campaigns_TenantId_BrandId",
                schema: "affiliate-service",
                table: "campaigns",
                columns: new[] { "TenantId", "BrandId" });

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_TenantId_Status",
                schema: "affiliate-service",
                table: "campaigns",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_campaign-boxes_CountryCode",
                schema: "affiliate-service",
                table: "campaign-boxes",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_boxes_TenantId_Name",
                schema: "affiliate-service",
                table: "boxes",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_partners_TenantId_GeneralInfo_Role",
                schema: "affiliate-service",
                table: "partners",
                columns: new[] { "TenantId", "GeneralInfo_Role" });

            migrationBuilder.CreateIndex(
                name: "IX_partners_TenantId_GeneralInfo_CreatedAt",
                schema: "affiliate-service",
                table: "partners",
                columns: new[] { "TenantId", "GeneralInfo_CreatedAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_campaigns_TenantId_BrandId",
                schema: "affiliate-service",
                table: "campaigns");

            migrationBuilder.DropIndex(
                name: "IX_campaigns_TenantId_Status",
                schema: "affiliate-service",
                table: "campaigns");

            migrationBuilder.DropIndex(
                name: "IX_campaign-boxes_CountryCode",
                schema: "affiliate-service",
                table: "campaign-boxes");

            migrationBuilder.DropIndex(
                name: "IX_boxes_TenantId_Name",
                schema: "affiliate-service",
                table: "boxes");

            migrationBuilder.DropIndex(
                name: "IX_partners_TenantId_GeneralInfo_Role",
                schema: "affiliate-service",
                table: "partners");

            migrationBuilder.DropIndex(
                name: "IX_partners_TenantId_GeneralInfo_CreatedAt",
                schema: "affiliate-service",
                table: "partners");
        }
    }
}
