using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "affiliate-service");

            migrationBuilder.CreateTable(
                name: "affiliates",
                schema: "affiliate-service",
                columns: table => new
                {
                    AffiliateId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    GeneralInfoUsername = table.Column<string>(type: "text", nullable: true),
                    GeneralInfoPassword = table.Column<string>(type: "text", nullable: true),
                    GeneralInfoEmail = table.Column<string>(type: "text", nullable: true),
                    GeneralInfoPhone = table.Column<string>(type: "text", nullable: true),
                    GeneralInfoSkype = table.Column<string>(type: "text", nullable: true),
                    GeneralInfoZipCode = table.Column<string>(type: "text", nullable: true),
                    GeneralInfoRole = table.Column<int>(type: "integer", nullable: false),
                    GeneralInfoState = table.Column<int>(type: "integer", nullable: false),
                    GeneralInfoCurrency = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    GeneralInfoApiKey = table.Column<string>(type: "text", nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    CompanyAddress = table.Column<string>(type: "text", nullable: true),
                    CompanyRegNumber = table.Column<string>(type: "text", nullable: true),
                    CompanyVatId = table.Column<string>(type: "text", nullable: true),
                    BankBeneficiaryName = table.Column<string>(type: "text", nullable: true),
                    BankBeneficiaryAddress = table.Column<string>(type: "text", nullable: true),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    BankAddress = table.Column<string>(type: "text", nullable: true),
                    BankAccountNumber = table.Column<string>(type: "text", nullable: true),
                    BankSwift = table.Column<string>(type: "text", nullable: true),
                    BankIban = table.Column<string>(type: "text", nullable: true),
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_affiliates", x => x.AffiliateId);
                });

            migrationBuilder.CreateTable(
                name: "campaigns",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "integrations",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_integrations", x => x.Id);
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
                    IntegrationId = table.Column<long>(type: "bigint", nullable: false),
                    Payout_Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    Payout_Currency = table.Column<int>(type: "integer", nullable: true),
                    Payout_Plan = table.Column<int>(type: "integer", nullable: true),
                    Revenue_Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    Revenue_Currency = table.Column<int>(type: "integer", nullable: true),
                    Revenue_Plan = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Privacy = table.Column<int>(type: "integer", nullable: false),
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_brands_integrations_IntegrationId",
                        column: x => x.IntegrationId,
                        principalSchema: "affiliate-service",
                        principalTable: "integrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaign-rows",
                schema: "affiliate-service",
                columns: table => new
                {
                    CampaignBoxId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CampaignId = table.Column<long>(type: "bigint", nullable: false),
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
                    CountryCode = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    CapType = table.Column<int>(type: "integer", nullable: false),
                    DailyCapValue = table.Column<long>(type: "bigint", nullable: false),
                    ActivityHours = table.Column<string>(type: "text", nullable: true),
                    Information = table.Column<string>(type: "text", nullable: true),
                    EnableTraffic = table.Column<bool>(type: "boolean", nullable: false),
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign-rows", x => x.CampaignBoxId);
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_TenantId_AffiliateId",
                schema: "affiliate-service",
                table: "affiliates",
                columns: new[] { "TenantId", "AffiliateId" });

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_TenantId_CreatedAt",
                schema: "affiliate-service",
                table: "affiliates",
                columns: new[] { "TenantId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_TenantId_GeneralInfoEmail",
                schema: "affiliate-service",
                table: "affiliates",
                columns: new[] { "TenantId", "GeneralInfoEmail" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_TenantId_GeneralInfoRole",
                schema: "affiliate-service",
                table: "affiliates",
                columns: new[] { "TenantId", "GeneralInfoRole" });

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_TenantId_GeneralInfoUsername",
                schema: "affiliate-service",
                table: "affiliates",
                columns: new[] { "TenantId", "GeneralInfoUsername" },
                unique: true);

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
                name: "IX_brands_TenantId_IntegrationId",
                schema: "affiliate-service",
                table: "brands",
                columns: new[] { "TenantId", "IntegrationId" });

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
                name: "IX_campaign-rows_CountryCode",
                schema: "affiliate-service",
                table: "campaign-rows",
                column: "CountryCode");

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
                name: "IX_integrations_TenantId_Id",
                schema: "affiliate-service",
                table: "integrations",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_integrations_TenantId_Name",
                schema: "affiliate-service",
                table: "integrations",
                columns: new[] { "TenantId", "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "affiliates",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "campaign-rows",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "brands",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "campaigns",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "integrations",
                schema: "affiliate-service");
        }
    }
}
