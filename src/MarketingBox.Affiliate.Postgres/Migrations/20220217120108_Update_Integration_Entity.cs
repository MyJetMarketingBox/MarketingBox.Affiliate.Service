using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class Update_Integration_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AffiliateId",
                schema: "affiliate-service",
                table: "integrations",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IntegrationType",
                schema: "affiliate-service",
                table: "integrations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "OfferId",
                schema: "affiliate-service",
                table: "integrations",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_offers_BrnadId",
                schema: "affiliate-service",
                table: "offers",
                column: "BrnadId");

            migrationBuilder.CreateIndex(
                name: "IX_integrations_AffiliateId",
                schema: "affiliate-service",
                table: "integrations",
                column: "AffiliateId");

            migrationBuilder.CreateIndex(
                name: "IX_integrations_IntegrationType",
                schema: "affiliate-service",
                table: "integrations",
                column: "IntegrationType");

            migrationBuilder.CreateIndex(
                name: "IX_integrations_OfferId",
                schema: "affiliate-service",
                table: "integrations",
                column: "OfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_integrations_affiliates_AffiliateId",
                schema: "affiliate-service",
                table: "integrations",
                column: "AffiliateId",
                principalSchema: "affiliate-service",
                principalTable: "affiliates",
                principalColumn: "AffiliateId");

            migrationBuilder.AddForeignKey(
                name: "FK_integrations_offers_OfferId",
                schema: "affiliate-service",
                table: "integrations",
                column: "OfferId",
                principalSchema: "affiliate-service",
                principalTable: "offers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_offers_brands_BrnadId",
                schema: "affiliate-service",
                table: "offers",
                column: "BrnadId",
                principalSchema: "affiliate-service",
                principalTable: "brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_integrations_affiliates_AffiliateId",
                schema: "affiliate-service",
                table: "integrations");

            migrationBuilder.DropForeignKey(
                name: "FK_integrations_offers_OfferId",
                schema: "affiliate-service",
                table: "integrations");

            migrationBuilder.DropForeignKey(
                name: "FK_offers_brands_BrnadId",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "IX_offers_BrnadId",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "IX_integrations_AffiliateId",
                schema: "affiliate-service",
                table: "integrations");

            migrationBuilder.DropIndex(
                name: "IX_integrations_IntegrationType",
                schema: "affiliate-service",
                table: "integrations");

            migrationBuilder.DropIndex(
                name: "IX_integrations_OfferId",
                schema: "affiliate-service",
                table: "integrations");

            migrationBuilder.DropColumn(
                name: "AffiliateId",
                schema: "affiliate-service",
                table: "integrations");

            migrationBuilder.DropColumn(
                name: "IntegrationType",
                schema: "affiliate-service",
                table: "integrations");

            migrationBuilder.DropColumn(
                name: "OfferId",
                schema: "affiliate-service",
                table: "integrations");
        }
    }
}
