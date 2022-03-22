using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class New_schema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "affiliate-service");

            migrationBuilder.CreateTable(
                name: "affiliate-subparam",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AffiliateId = table.Column<long>(type: "bigint", nullable: false),
                    ParamName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ParamValue = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_affiliate-subparam", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "banks",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BeneficiaryName = table.Column<string>(type: "text", nullable: true),
                    BeneficiaryAddress = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    AccountNumber = table.Column<string>(type: "text", nullable: true),
                    Swift = table.Column<string>(type: "text", nullable: true),
                    Iban = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "campaigns",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "companies",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    RegNumber = table.Column<string>(type: "text", nullable: true),
                    VatId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Numeric = table.Column<string>(type: "text", nullable: false),
                    Alfa2Code = table.Column<string>(type: "text", nullable: false),
                    Alfa3Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "geos",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CountryIds = table.Column<int[]>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "integrations",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_integrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "offers",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "affiliates",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Skype = table.Column<string>(type: "text", nullable: true),
                    ZipCode = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    ApiKey = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    BankId = table.Column<long>(type: "bigint", nullable: false),
                    LandingUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_affiliates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_affiliates_banks_BankId",
                        column: x => x.BankId,
                        principalSchema: "affiliate-service",
                        principalTable: "banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_affiliates_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "affiliate-service",
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "affiliate-payouts",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    PayoutType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_affiliate-payouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_affiliate-payouts_geos_GeoId",
                        column: x => x.GeoId,
                        principalSchema: "affiliate-service",
                        principalTable: "geos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brand-payouts",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    PayoutType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brand-payouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_brand-payouts_geos_GeoId",
                        column: x => x.GeoId,
                        principalSchema: "affiliate-service",
                        principalTable: "geos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brands",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    IntegrationId = table.Column<long>(type: "bigint", nullable: true),
                    IntegrationType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Privacy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_brands_integrations_IntegrationId",
                        column: x => x.IntegrationId,
                        principalSchema: "affiliate-service",
                        principalTable: "integrations",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateTable(
                name: "offer-affiliates",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<long>(type: "bigint", nullable: false),
                    AffiliateId = table.Column<long>(type: "bigint", nullable: false),
                    OfferId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offer-affiliates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_offer-affiliates_affiliates_AffiliateId",
                        column: x => x.AffiliateId,
                        principalSchema: "affiliate-service",
                        principalTable: "affiliates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_offer-affiliates_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalSchema: "affiliate-service",
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_offer-affiliates_offers_OfferId",
                        column: x => x.OfferId,
                        principalSchema: "affiliate-service",
                        principalTable: "offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AffiliateAffiliatePayout",
                schema: "affiliate-service",
                columns: table => new
                {
                    AffiliatesId = table.Column<long>(type: "bigint", nullable: false),
                    PayoutsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliateAffiliatePayout", x => new { x.AffiliatesId, x.PayoutsId });
                    table.ForeignKey(
                        name: "FK_AffiliateAffiliatePayout_affiliate-payouts_PayoutsId",
                        column: x => x.PayoutsId,
                        principalSchema: "affiliate-service",
                        principalTable: "affiliate-payouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AffiliateAffiliatePayout_affiliates_AffiliatesId",
                        column: x => x.AffiliatesId,
                        principalSchema: "affiliate-service",
                        principalTable: "affiliates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrandBrandPayout",
                schema: "affiliate-service",
                columns: table => new
                {
                    BrandsId = table.Column<long>(type: "bigint", nullable: false),
                    PayoutsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandBrandPayout", x => new { x.BrandsId, x.PayoutsId });
                    table.ForeignKey(
                        name: "FK_BrandBrandPayout_brand-payouts_PayoutsId",
                        column: x => x.PayoutsId,
                        principalSchema: "affiliate-service",
                        principalTable: "brand-payouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandBrandPayout_brands_BrandsId",
                        column: x => x.BrandsId,
                        principalSchema: "affiliate-service",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaign-rows",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<long>(type: "bigint", nullable: false),
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
                    GeoId = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    CapType = table.Column<int>(type: "integer", nullable: false),
                    DailyCapValue = table.Column<long>(type: "bigint", nullable: false),
                    ActivityHours = table.Column<string>(type: "text", nullable: true),
                    Information = table.Column<string>(type: "text", nullable: true),
                    EnableTraffic = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign-rows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_campaign-rows_brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "affiliate-service",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_campaign-rows_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalSchema: "affiliate-service",
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_campaign-rows_geos_GeoId",
                        column: x => x.GeoId,
                        principalSchema: "affiliate-service",
                        principalTable: "geos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_affiliate-payouts_CreatedAt",
                schema: "affiliate-service",
                table: "affiliate-payouts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_affiliate-payouts_Currency",
                schema: "affiliate-service",
                table: "affiliate-payouts",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_affiliate-payouts_GeoId",
                schema: "affiliate-service",
                table: "affiliate-payouts",
                column: "GeoId");

            migrationBuilder.CreateIndex(
                name: "IX_affiliate-payouts_ModifiedAt",
                schema: "affiliate-service",
                table: "affiliate-payouts",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_affiliate-payouts_PayoutType",
                schema: "affiliate-service",
                table: "affiliate-payouts",
                column: "PayoutType");

            migrationBuilder.CreateIndex(
                name: "IX_AffiliateAffiliatePayout_PayoutsId",
                schema: "affiliate-service",
                table: "AffiliateAffiliatePayout",
                column: "PayoutsId");

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_BankId",
                schema: "affiliate-service",
                table: "affiliates",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_CompanyId",
                schema: "affiliate-service",
                table: "affiliates",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_TenantId_CreatedAt",
                schema: "affiliate-service",
                table: "affiliates",
                columns: new[] { "TenantId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_TenantId_Email",
                schema: "affiliate-service",
                table: "affiliates",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_TenantId_Id",
                schema: "affiliate-service",
                table: "affiliates",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_TenantId_Username",
                schema: "affiliate-service",
                table: "affiliates",
                columns: new[] { "TenantId", "Username" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_banks_Name",
                schema: "affiliate-service",
                table: "banks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_brand-payouts_CreatedAt",
                schema: "affiliate-service",
                table: "brand-payouts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_brand-payouts_Currency",
                schema: "affiliate-service",
                table: "brand-payouts",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_brand-payouts_GeoId",
                schema: "affiliate-service",
                table: "brand-payouts",
                column: "GeoId");

            migrationBuilder.CreateIndex(
                name: "IX_brand-payouts_ModifiedAt",
                schema: "affiliate-service",
                table: "brand-payouts",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_brand-payouts_PayoutType",
                schema: "affiliate-service",
                table: "brand-payouts",
                column: "PayoutType");

            migrationBuilder.CreateIndex(
                name: "IX_BrandBrandPayout_PayoutsId",
                schema: "affiliate-service",
                table: "BrandBrandPayout",
                column: "PayoutsId");

            migrationBuilder.CreateIndex(
                name: "IX_brands_IntegrationId",
                schema: "affiliate-service",
                table: "brands",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_brands_TenantId_Id",
                schema: "affiliate-service",
                table: "brands",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_brands_TenantId_Status",
                schema: "affiliate-service",
                table: "brands",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_campaign-rows_BrandId",
                schema: "affiliate-service",
                table: "campaign-rows",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_campaign-rows_CampaignId",
                schema: "affiliate-service",
                table: "campaign-rows",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_campaign-rows_GeoId",
                schema: "affiliate-service",
                table: "campaign-rows",
                column: "GeoId");

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_TenantId_Id",
                schema: "affiliate-service",
                table: "campaigns",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_TenantId_Name",
                schema: "affiliate-service",
                table: "campaigns",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_companies_Name",
                schema: "affiliate-service",
                table: "companies",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_countries_Alfa2Code",
                schema: "affiliate-service",
                table: "countries",
                column: "Alfa2Code");

            migrationBuilder.CreateIndex(
                name: "IX_countries_Alfa3Code",
                schema: "affiliate-service",
                table: "countries",
                column: "Alfa3Code");

            migrationBuilder.CreateIndex(
                name: "IX_countries_Name",
                schema: "affiliate-service",
                table: "countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_countries_Numeric",
                schema: "affiliate-service",
                table: "countries",
                column: "Numeric");

            migrationBuilder.CreateIndex(
                name: "IX_geos_CreatedAt",
                schema: "affiliate-service",
                table: "geos",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_geos_Name",
                schema: "affiliate-service",
                table: "geos",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_integrations_TenantId_Id",
                schema: "affiliate-service",
                table: "integrations",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_integrations_TenantId_Name",
                schema: "affiliate-service",
                table: "integrations",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_AffiliateId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                column: "AffiliateId");

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_CampaignId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_offer-affiliates_OfferId",
                schema: "affiliate-service",
                table: "offer-affiliates",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_offer-subparams_OfferId",
                schema: "affiliate-service",
                table: "offer-subparams",
                column: "OfferId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "affiliate-subparam",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "AffiliateAffiliatePayout",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "BrandBrandPayout",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "campaign-rows",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "countries",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "offer-affiliates",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "offer-subparams",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "affiliate-payouts",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "brand-payouts",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "brands",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "affiliates",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "campaigns",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "offers",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "geos",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "integrations",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "banks",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "companies",
                schema: "affiliate-service");
        }
    }
}
