using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class nullable_companyId_BankId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_affiliates_banks_BankId",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropForeignKey(
                name: "FK_affiliates_companies_CompanyId",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "affiliate-service",
                table: "affiliates",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "BankId",
                schema: "affiliate-service",
                table: "affiliates",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_affiliates_banks_BankId",
                schema: "affiliate-service",
                table: "affiliates",
                column: "BankId",
                principalSchema: "affiliate-service",
                principalTable: "banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_affiliates_companies_CompanyId",
                schema: "affiliate-service",
                table: "affiliates",
                column: "CompanyId",
                principalSchema: "affiliate-service",
                principalTable: "companies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_affiliates_banks_BankId",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropForeignKey(
                name: "FK_affiliates_companies_CompanyId",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "affiliate-service",
                table: "affiliates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BankId",
                schema: "affiliate-service",
                table: "affiliates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_affiliates_banks_BankId",
                schema: "affiliate-service",
                table: "affiliates",
                column: "BankId",
                principalSchema: "affiliate-service",
                principalTable: "banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_affiliates_companies_CompanyId",
                schema: "affiliate-service",
                table: "affiliates",
                column: "CompanyId",
                principalSchema: "affiliate-service",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
