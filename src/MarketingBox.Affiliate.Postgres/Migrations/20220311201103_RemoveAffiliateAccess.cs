using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class RemoveAffiliateAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "affiliate_access",
                schema: "affiliate-service");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "affiliate_access",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AffiliateId = table.Column<long>(type: "bigint", nullable: false),
                    MasterAffiliateId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_affiliate_access", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_affiliate_access_AffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access",
                column: "AffiliateId");

            migrationBuilder.CreateIndex(
                name: "IX_affiliate_access_MasterAffiliateId_AffiliateId",
                schema: "affiliate-service",
                table: "affiliate_access",
                columns: new[] { "MasterAffiliateId", "AffiliateId" },
                unique: true);
        }
    }
}
