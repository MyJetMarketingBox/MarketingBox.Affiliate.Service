using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class Campaign_createdAt_lastActiveAt_createdBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "affiliate-service",
                table: "campaigns",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatedById",
                schema: "affiliate-service",
                table: "campaigns",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveAt",
                schema: "affiliate-service",
                table: "campaigns",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "affiliate-service",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "affiliate-service",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "LastActiveAt",
                schema: "affiliate-service",
                table: "campaigns");
        }
    }
}
