using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class addNameToPayouts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "affiliate-service",
                table: "brand-payouts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "affiliate-service",
                table: "affiliate-payouts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "affiliate-service",
                table: "brand-payouts");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "affiliate-service",
                table: "affiliate-payouts");
        }
    }
}
