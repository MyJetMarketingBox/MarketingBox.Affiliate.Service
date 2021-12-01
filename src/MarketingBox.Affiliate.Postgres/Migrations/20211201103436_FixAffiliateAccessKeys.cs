using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class FixAffiliateAccessKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_affiliate_access_affiliates_AffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access");

            migrationBuilder.DropForeignKey(
                name: "FK_affiliate_access_affiliates_MasterAffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access");

            migrationBuilder.DropIndex(
                name: "IX_affiliate_access_AffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access");

            migrationBuilder.DropIndex(
                name: "IX_affiliate_access_MasterAffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access");

            migrationBuilder.AddColumn<long>(
                name: "AccessIsGivenById",
                schema: "affiliate-service",
                table: "affiliates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_affiliate_access_AffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access",
                column: "AffiliateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_affiliate_access_AffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access");

            migrationBuilder.DropColumn(
                name: "AccessIsGivenById",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.CreateIndex(
                name: "IX_affiliate_access_AffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access",
                column: "AffiliateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_affiliate_access_MasterAffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access",
                column: "MasterAffiliateId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_affiliate_access_affiliates_AffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access",
                column: "AffiliateId",
                principalSchema: "affiliate-service",
                principalTable: "affiliates",
                principalColumn: "AffiliateId");

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
