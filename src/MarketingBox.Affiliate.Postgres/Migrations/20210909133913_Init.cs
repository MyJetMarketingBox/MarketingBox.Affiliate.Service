using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "affiliate-service");

            migrationBuilder.CreateTable(
                name: "partners",
                schema: "affiliate-service",
                columns: table => new
                {
                    AffiliateId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    GeneralInfo_Username = table.Column<string>(type: "text", nullable: true),
                    GeneralInfo_Password = table.Column<string>(type: "text", nullable: true),
                    GeneralInfo_Email = table.Column<string>(type: "text", nullable: true),
                    GeneralInfo_Phone = table.Column<string>(type: "text", nullable: true),
                    GeneralInfo_Skype = table.Column<string>(type: "text", nullable: true),
                    GeneralInfo_ZipCode = table.Column<string>(type: "text", nullable: true),
                    GeneralInfo_Role = table.Column<int>(type: "integer", nullable: true),
                    GeneralInfo_State = table.Column<int>(type: "integer", nullable: true),
                    GeneralInfo_Currency = table.Column<int>(type: "integer", nullable: true),
                    GeneralInfo_CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Company_Name = table.Column<string>(type: "text", nullable: true),
                    Company_Address = table.Column<string>(type: "text", nullable: true),
                    Company_RegNumber = table.Column<string>(type: "text", nullable: true),
                    Company_VatId = table.Column<string>(type: "text", nullable: true),
                    Bank_BeneficiaryName = table.Column<string>(type: "text", nullable: true),
                    Bank_BeneficiaryAddress = table.Column<string>(type: "text", nullable: true),
                    Bank_BankName = table.Column<string>(type: "text", nullable: true),
                    Bank_BankAddress = table.Column<string>(type: "text", nullable: true),
                    Bank_AccountNumber = table.Column<string>(type: "text", nullable: true),
                    Bank_Swift = table.Column<string>(type: "text", nullable: true),
                    Bank_Iban = table.Column<string>(type: "text", nullable: true),
                    Sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_partners", x => x.AffiliateId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_partners_TenantId_AffiliateId",
                schema: "affiliate-service",
                table: "partners",
                columns: new[] { "TenantId", "AffiliateId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "partners",
                schema: "affiliate-service");
        }
    }
}
