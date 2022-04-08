using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class remove_proxyLink_from : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProxyLink",
                schema: "affiliate-service",
                table: "offer-affiliates");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProxyLink",
                schema: "affiliate-service",
                table: "offer-affiliates",
                type: "text",
                nullable: true);
        }
    }
}
