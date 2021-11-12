using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class AffiliateAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "affiliate_access",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MasterAffiliateId = table.Column<long>(type: "bigint", nullable: false),
                    AffiliateId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_affiliate_access", x => x.Id);
                    table.ForeignKey(
                        name: "FK_affiliate_access_affiliates_AffiliateId",
                        column: x => x.AffiliateId,
                        principalSchema: "affiliate-service",
                        principalTable: "affiliates",
                        principalColumn: "AffiliateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_affiliate_access_affiliates_MasterAffiliateId",
                        column: x => x.MasterAffiliateId,
                        principalSchema: "affiliate-service",
                        principalTable: "affiliates",
                        principalColumn: "AffiliateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_affiliate_access_AffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access",
                column: "AffiliateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_affiliate_access_MasterAffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access",
                column: "MasterAffiliateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_affiliate_access_MasterAffiliateId_AffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access",
                columns: new[] { "MasterAffiliateId", "AffiliateId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "affiliate_access",
                schema: "affiliate-service");
        }
    }
}
