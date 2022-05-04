using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class remove_campaign_from_offeraffiliates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_offer-affiliates_campaigns_CampaignId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.DropIndex(
                name: "IX_offer-affiliates_CampaignId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                schema: "affiliate-service",
                table: "offer-affiliates");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CampaignId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_CampaignId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_offer-affiliates_campaigns_CampaignId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                column: "CampaignId",
                principalSchema: "affiliate-service",
                principalTable: "campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
