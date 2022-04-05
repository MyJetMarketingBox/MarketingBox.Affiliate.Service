using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class Offer_OfferAffiliate_Brand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "offer-subparams",
                schema: "affiliate-service");

            migrationBuilder.DropIndex(
                name: "IX_brands_TenantId_Status",
                schema: "affiliate-service",
                table: "brands");

            migrationBuilder.DropColumn(
                name: "Privacy",
                schema: "affiliate-service",
                table: "brands");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "affiliate-service",
                table: "brands");

            migrationBuilder.AddColumn<long>(
                name: "BrandId",
                schema: "affiliate-service",
                table: "offers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Currency",
                schema: "affiliate-service",
                table: "offers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                schema: "affiliate-service",
                table: "offers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Privacy",
                schema: "affiliate-service",
                table: "offers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                schema: "affiliate-service",
                table: "offers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProxyLink",
                schema: "affiliate-service",
                table: "offer-affiliates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Link",
                schema: "affiliate-service",
                table: "brands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkParameters_ClickId",
                schema: "affiliate-service",
                table: "brands",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkParameters_Language",
                schema: "affiliate-service",
                table: "brands",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkParameters_MPC_1",
                schema: "affiliate-service",
                table: "brands",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkParameters_MPC_2",
                schema: "affiliate-service",
                table: "brands",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkParameters_MPC_3",
                schema: "affiliate-service",
                table: "brands",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkParameters_MPC_4",
                schema: "affiliate-service",
                table: "brands",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GeoOffer",
                schema: "affiliate-service",
                columns: table => new
                {
                    GeosId = table.Column<int>(type: "integer", nullable: false),
                    OffersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoOffer", x => new { x.GeosId, x.OffersId });
                    table.ForeignKey(
                        name: "FK_GeoOffer_geos_GeosId",
                        column: x => x.GeosId,
                        principalSchema: "affiliate-service",
                        principalTable: "geos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeoOffer_offers_OffersId",
                        column: x => x.OffersId,
                        principalSchema: "affiliate-service",
                        principalTable: "offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code2 = table.Column<string>(type: "text", nullable: false),
                    Code3 = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_languages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_offers_BrandId",
                schema: "affiliate-service",
                table: "offers",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_offers_LanguageId",
                schema: "affiliate-service",
                table: "offers",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_GeoOffer_OffersId",
                schema: "affiliate-service",
                table: "GeoOffer",
                column: "OffersId");

            migrationBuilder.CreateIndex(
                name: "IX_languages_Code2",
                schema: "affiliate-service",
                table: "languages",
                column: "Code2");

            migrationBuilder.CreateIndex(
                name: "IX_languages_Code3",
                schema: "affiliate-service",
                table: "languages",
                column: "Code3");

            migrationBuilder.CreateIndex(
                name: "IX_languages_Name",
                schema: "affiliate-service",
                table: "languages",
                column: "Name",
                unique: true);

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

            migrationBuilder.DropTable(
                name: "GeoOffer",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "languages",
                schema: "affiliate-service");

            migrationBuilder.DropIndex(
                name: "IX_offers_BrandId",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "IX_offers_LanguageId",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "BrandId",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "Privacy",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "State",
                schema: "affiliate-service",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "ProxyLink",
                schema: "affiliate-service",
                table: "offer-affiliates");

            migrationBuilder.DropColumn(
                name: "Link",
                schema: "affiliate-service",
                table: "brands");

            migrationBuilder.DropColumn(
                name: "LinkParameters_ClickId",
                schema: "affiliate-service",
                table: "brands");

            migrationBuilder.DropColumn(
                name: "LinkParameters_Language",
                schema: "affiliate-service",
                table: "brands");

            migrationBuilder.DropColumn(
                name: "LinkParameters_MPC_1",
                schema: "affiliate-service",
                table: "brands");

            migrationBuilder.DropColumn(
                name: "LinkParameters_MPC_2",
                schema: "affiliate-service",
                table: "brands");

            migrationBuilder.DropColumn(
                name: "LinkParameters_MPC_3",
                schema: "affiliate-service",
                table: "brands");

            migrationBuilder.DropColumn(
                name: "LinkParameters_MPC_4",
                schema: "affiliate-service",
                table: "brands");

            migrationBuilder.AddColumn<int>(
                name: "Privacy",
                schema: "affiliate-service",
                table: "brands",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "affiliate-service",
                table: "brands",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "offer-subparams",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OfferId = table.Column<long>(type: "bigint", nullable: false),
                    ParamName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ParamValue = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offer-subparams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_offer-subparams_offers_OfferId",
                        column: x => x.OfferId,
                        principalSchema: "affiliate-service",
                        principalTable: "offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_brands_TenantId_Status",
                schema: "affiliate-service",
                table: "brands",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_offer-subparams_OfferId",
                schema: "affiliate-service",
                table: "offer-subparams",
                column: "OfferId");
        }
    }
}
