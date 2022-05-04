using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class unique_indices_for_offerId_affiliateId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_offer-affiliates_OfferId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.DropIndex(
                name: "IX_offer-affiliates_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_OfferId_AffiliateId_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                columns: new[] { "OfferId", "AffiliateId", "UniqueId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_offer-affiliates_OfferId_AffiliateId_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_OfferId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                column: "UniqueId",
                unique: true);
        }
    }
}
