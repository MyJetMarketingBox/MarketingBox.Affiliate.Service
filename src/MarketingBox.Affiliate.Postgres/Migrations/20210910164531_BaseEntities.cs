using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class BaseEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "boxes",
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
                    table.PrimaryKey("PK_boxes", x => x.Id);
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
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.Id);
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
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_campaigns_brands_BrandId",
                        column: x => x.BrandId,
                        principalSchema: "affiliate-service",
                        principalTable: "brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaign-boxes",
                schema: "affiliate-service",
                columns: table => new
                {
                    CampaignBoxId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BoxId = table.Column<long>(type: "bigint", nullable: false),
                    CampaignId = table.Column<long>(type: "bigint", nullable: false),
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
                    table.PrimaryKey("PK_campaign-boxes", x => x.CampaignBoxId);
                    table.ForeignKey(
                        name: "FK_campaign-boxes_boxes_BoxId",
                        column: x => x.BoxId,
                        principalSchema: "affiliate-service",
                        principalTable: "boxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_campaign-boxes_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalSchema: "affiliate-service",
                        principalTable: "campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_boxes_TenantId_Id",
                schema: "affiliate-service",
                table: "boxes",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_brands_TenantId_Id",
                schema: "affiliate-service",
                table: "brands",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_campaign-boxes_BoxId",
                schema: "affiliate-service",
                table: "campaign-boxes",
                column: "BoxId");

            migrationBuilder.CreateIndex(
                name: "IX_campaign-boxes_CampaignId",
                schema: "affiliate-service",
                table: "campaign-boxes",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_BrandId",
                schema: "affiliate-service",
                table: "campaigns",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_TenantId_Id",
                schema: "affiliate-service",
                table: "campaigns",
                columns: new[] { "TenantId", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign-boxes",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "boxes",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "campaigns",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "brands",
                schema: "affiliate-service");
        }
    }
}
