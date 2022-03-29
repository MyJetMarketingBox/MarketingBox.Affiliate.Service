using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class own_types_Bank_Company : Migration
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

            migrationBuilder.DropTable(
                name: "banks",
                schema: "affiliate-service");

            migrationBuilder.DropTable(
                name: "companies",
                schema: "affiliate-service");

            migrationBuilder.DropIndex(
                name: "IX_affiliates_BankId",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropIndex(
                name: "IX_affiliates_CompanyId",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropColumn(
                name: "BankId",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.AddColumn<string>(
                name: "Bank_AccountNumber",
                schema: "affiliate-service",
                table: "affiliates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bank_Address",
                schema: "affiliate-service",
                table: "affiliates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bank_BeneficiaryAddress",
                schema: "affiliate-service",
                table: "affiliates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bank_BeneficiaryName",
                schema: "affiliate-service",
                table: "affiliates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bank_Iban",
                schema: "affiliate-service",
                table: "affiliates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bank_Name",
                schema: "affiliate-service",
                table: "affiliates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bank_Swift",
                schema: "affiliate-service",
                table: "affiliates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company_Address",
                schema: "affiliate-service",
                table: "affiliates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company_Name",
                schema: "affiliate-service",
                table: "affiliates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company_RegNumber",
                schema: "affiliate-service",
                table: "affiliates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company_VatId",
                schema: "affiliate-service",
                table: "affiliates",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bank_AccountNumber",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropColumn(
                name: "Bank_Address",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropColumn(
                name: "Bank_BeneficiaryAddress",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropColumn(
                name: "Bank_BeneficiaryName",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropColumn(
                name: "Bank_Iban",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropColumn(
                name: "Bank_Name",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropColumn(
                name: "Bank_Swift",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropColumn(
                name: "Company_Address",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropColumn(
                name: "Company_Name",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropColumn(
                name: "Company_RegNumber",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.DropColumn(
                name: "Company_VatId",
                schema: "affiliate-service",
                table: "affiliates");

            migrationBuilder.AddColumn<long>(
                name: "BankId",
                schema: "affiliate-service",
                table: "affiliates",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                schema: "affiliate-service",
                table: "affiliates",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "banks",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountNumber = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    BeneficiaryAddress = table.Column<string>(type: "text", nullable: true),
                    BeneficiaryName = table.Column<string>(type: "text", nullable: true),
                    Iban = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Swift = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "companies",
                schema: "affiliate-service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RegNumber = table.Column<string>(type: "text", nullable: true),
                    VatId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_BankId",
                schema: "affiliate-service",
                table: "affiliates",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_affiliates_CompanyId",
                schema: "affiliate-service",
                table: "affiliates",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_banks_Name",
                schema: "affiliate-service",
                table: "banks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_companies_Name",
                schema: "affiliate-service",
                table: "companies",
                column: "Name",
                unique: true);

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
    }
}
