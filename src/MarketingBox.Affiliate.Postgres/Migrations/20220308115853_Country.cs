using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class Country : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_campaign-rows_CountryCode",
                schema: "affiliate-service",
                table: "campaign-rows");

            migrationBuilder.DropIndex(
                name: "IX_brands_TenantId_IntegrationId",
                schema: "affiliate-service",
                table: "brands");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                schema: "affiliate-service",
                table: "campaign-rows");

            migrationBuilder.DropColumn(
                name: "Sequence",
                schema: "affiliate-service",
                table: "campaign-rows");

            migrationBuilder.AddColumn<int>(
                name: "GeoId",
                schema: "affiliate-service",
                table: "campaign-rows",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "countries",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ISOCode = table.Column<string>(type: "text", nullable: false),
                    Alfa2Code = table.Column<string>(type: "text", nullable: false),
                    Alfa3Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "geos",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CountryIds = table.Column<int[]>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campaign-rows_GeoId",
                schema: "affiliate-service",
                table: "campaign-rows",
                column: "GeoId");

            migrationBuilder.CreateIndex(
                name: "IX_countries_Name",
                schema: "affiliate-service",
                table: "countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_geos_CreatedAt",
                schema: "affiliate-service",
                table: "geos",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_geos_Name",
                schema: "affiliate-service",
                table: "geos",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_campaign-rows_geos_GeoId",
                schema: "affiliate-service",
                table: "campaign-rows",
                column: "GeoId",
                principalSchema: "affiliate-service",
                principalTable: "geos",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_campaign-rows_geos_GeoId",
                schema: "affiliate-service",
                table: "campaign-rows");

            migrationBuilder.DropTable(
                name: "countries",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "geos",
                schema: "affiliate-service");

            migrationBuilder.DropIndex(
                name: "IX_campaign-rows_GeoId",
                schema: "affiliate-service",
                table: "campaign-rows");

            migrationBuilder.DropColumn(
                name: "GeoId",
                schema: "affiliate-service",
                table: "campaign-rows");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                schema: "affiliate-service",
                table: "campaign-rows",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Sequence",
                schema: "affiliate-service",
                table: "campaign-rows",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_campaign-rows_CountryCode",
                schema: "affiliate-service",
                table: "campaign-rows",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_brands_TenantId_IntegrationId",
                schema: "affiliate-service",
                table: "brands",
                columns: new[] { "TenantId", "IntegrationId" });
        }
    }
}
