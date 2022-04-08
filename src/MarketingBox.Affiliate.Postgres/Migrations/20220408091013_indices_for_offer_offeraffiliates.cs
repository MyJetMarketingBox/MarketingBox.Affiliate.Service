using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class indices_for_offer_offeraffiliates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_offers_brands_BrandId",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropForeignKey(
                name: "FK_offers_languages_LanguageId",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.AlterColumn<int>(
                name: "LanguageId",
                schema: "affiliate-service",
                table: "offers",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BrandId",
                schema: "affiliate-service",
                table: "offers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_offers_Currency",
                schema: "affiliate-service",
                table: "offers",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_offers_Name",
                schema: "affiliate-service",
                table: "offers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_offers_Privacy",
                schema: "affiliate-service",
                table: "offers",
                column: "Privacy");

            migrationBuilder.CreateIndex(
                name: "IX_offers_State",
                schema: "affiliate-service",
                table: "offers",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_brands_IntegrationType",
                schema: "affiliate-service",
                table: "brands",
                column: "IntegrationType");

            migrationBuilder.AddForeignKey(
                name: "FK_offers_brands_BrandId",
                schema: "affiliate-service",
                table: "offers",
                column: "BrandId",
                principalSchema: "affiliate-service",
                principalTable: "brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_offers_languages_LanguageId",
                schema: "affiliate-service",
                table: "offers",
                column: "LanguageId",
                principalSchema: "affiliate-service",
                principalTable: "languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_offers_brands_BrandId",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropForeignKey(
                name: "FK_offers_languages_LanguageId",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "IX_offers_Currency",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "IX_offers_Name",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "IX_offers_Privacy",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "IX_offers_State",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "IX_offer-affiliates_UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.DropIndex(
                name: "IX_brands_IntegrationType",
                schema: "affiliate-service",
                table: "brands");

            migrationBuilder.DropColumn(
                name: "UniqueId",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.AlterColumn<int>(
                name: "LanguageId",
                schema: "affiliate-service",
                table: "offers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "BrandId",
                schema: "affiliate-service",
                table: "offers",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_offers_brands_BrandId",
                schema: "affiliate-service",
                table: "offers",
                column: "BrandId",
                principalSchema: "affiliate-service",
                principalTable: "brands",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_offers_languages_LanguageId",
                schema: "affiliate-service",
                table: "offers",
                column: "LanguageId",
                principalSchema: "affiliate-service",
                principalTable: "languages",
                principalColumn: "Id");
        }
    }
}
