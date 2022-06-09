using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class MultiTenancy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_offer-affiliates_OfferId_AffiliateId_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.DropIndex(
                name: "IX_geos_Name",
                schema: "affiliate-service",
                table: "geos");

            migrationBuilder.DropIndex(
                name: "IX_brandboxes_Name",
                schema: "affiliate-service",
                table: "brandboxes");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "affiliate-service",
                table: "offers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "affiliate-service",
                table: "geos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "affiliate-service",
                table: "campaign-rows",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "affiliate-service",
                table: "brandboxes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "affiliate-service",
                table: "brand-payouts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "affiliate-service",
                table: "affiliate-subparam",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "affiliate-service",
                table: "affiliate-payouts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_offers_TenantId_Id",
                schema: "affiliate-service",
                table: "offers",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_OfferId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_TenantId_Id",
                schema: "affiliate-service",
                table: "offer-affiliates",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_TenantId_OfferId_AffiliateId_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                columns: new[] { "TenantId", "OfferId", "AffiliateId", "UniqueId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_geos_TenantId_Id",
                schema: "affiliate-service",
                table: "geos",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_geos_TenantId_Name",
                schema: "affiliate-service",
                table: "geos",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_campaign-rows_TenantId_Id",
                schema: "affiliate-service",
                table: "campaign-rows",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_brandboxes_TenantId_Id",
                schema: "affiliate-service",
                table: "brandboxes",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_brandboxes_TenantId_Name",
                schema: "affiliate-service",
                table: "brandboxes",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_brand-payouts_TenantId_Id",
                schema: "affiliate-service",
                table: "brand-payouts",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_affiliate-subparam_TenantId_Id",
                schema: "affiliate-service",
                table: "affiliate-subparam",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_affiliate-payouts_TenantId_Id",
                schema: "affiliate-service",
                table: "affiliate-payouts",
                columns: new[] { "TenantId", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_offers_TenantId_Id",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "IX_offer-affiliates_OfferId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.DropIndex(
                name: "IX_offer-affiliates_TenantId_Id",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.DropIndex(
                name: "IX_offer-affiliates_TenantId_OfferId_AffiliateId_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.DropIndex(
                name: "IX_geos_TenantId_Id",
                schema: "affiliate-service",
                table: "geos");

            migrationBuilder.DropIndex(
                name: "IX_geos_TenantId_Name",
                schema: "affiliate-service",
                table: "geos");

            migrationBuilder.DropIndex(
                name: "IX_campaign-rows_TenantId_Id",
                schema: "affiliate-service",
                table: "campaign-rows");

            migrationBuilder.DropIndex(
                name: "IX_brandboxes_TenantId_Id",
                schema: "affiliate-service",
                table: "brandboxes");

            migrationBuilder.DropIndex(
                name: "IX_brandboxes_TenantId_Name",
                schema: "affiliate-service",
                table: "brandboxes");

            migrationBuilder.DropIndex(
                name: "IX_brand-payouts_TenantId_Id",
                schema: "affiliate-service",
                table: "brand-payouts");

            migrationBuilder.DropIndex(
                name: "IX_affiliate-subparam_TenantId_Id",
                schema: "affiliate-service",
                table: "affiliate-subparam");

            migrationBuilder.DropIndex(
                name: "IX_affiliate-payouts_TenantId_Id",
                schema: "affiliate-service",
                table: "affiliate-payouts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "affiliate-service",
                table: "geos");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "affiliate-service",
                table: "campaign-rows");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "affiliate-service",
                table: "brandboxes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "affiliate-service",
                table: "brand-payouts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "affiliate-service",
                table: "affiliate-subparam");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "affiliate-service",
                table: "affiliate-payouts");

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_OfferId_AffiliateId_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                columns: new[] { "OfferId", "AffiliateId", "UniqueId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_geos_Name",
                schema: "affiliate-service",
                table: "geos",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_brandboxes_Name",
                schema: "affiliate-service",
                table: "brandboxes",
                column: "Name",
                unique: true);
        }
    }
}
