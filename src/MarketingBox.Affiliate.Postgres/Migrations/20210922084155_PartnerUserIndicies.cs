using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class PartnerUserIndicies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_partners_TenantId_GeneralInfo_Email",
                schema: "affiliate-service",
                table: "partners",
                columns: new[] { "TenantId", "GeneralInfo_Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_partners_TenantId_GeneralInfo_Username",
                schema: "affiliate-service",
                table: "partners",
                columns: new[] { "TenantId", "GeneralInfo_Username" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_partners_TenantId_GeneralInfo_Email",
                schema: "affiliate-service",
                table: "partners");

            migrationBuilder.DropIndex(
                name: "IX_partners_TenantId_GeneralInfo_Username",
                schema: "affiliate-service",
                table: "partners");
        }
    }
}
