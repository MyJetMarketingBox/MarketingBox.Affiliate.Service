using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class offer_createdBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_offer-affiliates_OfferId_AffiliateId_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                schema: "affiliate-service",
                table: "offers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_OfferId_AffiliateId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                columns: new[] { "OfferId", "AffiliateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                column: "UniqueId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_offer-affiliates_OfferId_AffiliateId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.DropIndex(
                name: "IX_offer-affiliates_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_OfferId_AffiliateId_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                columns: new[] { "OfferId", "AffiliateId", "UniqueId" },
                unique: true);
        }
    }
}
