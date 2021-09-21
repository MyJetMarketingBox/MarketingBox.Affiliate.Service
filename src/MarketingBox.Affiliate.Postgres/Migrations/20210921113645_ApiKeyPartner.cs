using Microsoft.EntityFrameworkCore.Migrations;

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class ApiKeyPartner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GeneralInfo_ApiKey",
                schema: "affiliate-service",
                table: "partners",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneralInfo_ApiKey",
                schema: "affiliate-service",
                table: "partners");
        }
    }
}
