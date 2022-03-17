using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class affiliates_CreatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LandingUrl",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                schema: "affiliate-service",
                table: "affiliates",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.AddColumn<string>(
                name: "LandingUrl",
                schema: "affiliate-service",
                table: "affiliates",
                type: "text",
                nullable: true);
        }
    }
}
