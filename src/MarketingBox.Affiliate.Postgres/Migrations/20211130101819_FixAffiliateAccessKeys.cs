using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class FixAffiliateAccessKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_affiliate_access_affiliates_MasterAffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access");

            migrationBuilder.AddForeignKey(
                name: "FK_affiliate_access_affiliates_MasterAffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access",
                column: "MasterAffiliateId",
                principalSchema: "affiliate-service",
                principalTable: "affiliates",
                principalColumn: "AffiliateId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_affiliate_access_affiliates_MasterAffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access");

            migrationBuilder.AddForeignKey(
                name: "FK_affiliate_access_affiliates_MasterAffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access",
                column: "MasterAffiliateId",
                principalSchema: "affiliate-service",
                principalTable: "affiliates",
                principalColumn: "AffiliateId");
        }
    }
}
