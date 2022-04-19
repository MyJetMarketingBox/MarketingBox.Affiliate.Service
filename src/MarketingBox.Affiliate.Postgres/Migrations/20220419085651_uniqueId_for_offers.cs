using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class uniqueId_for_offers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UniqueId",
                schema: "affiliate-service",
                table: "offers",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UniqueId",
                schema: "affiliate-service",
                table: "offers");
        }
    }
}
