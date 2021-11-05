﻿// <auto-generated />
using System;
using MarketingBox.Affiliate.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20211105145057_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("affiliate-service")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("MarketingBox.Affiliate.Postgres.Entities.Brands.BrandEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("IntegrationId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Privacy")
                        .HasColumnType("integer");

                    b.Property<long>("Sequence")
                        .HasColumnType("bigint");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("IntegrationId");

                    b.HasIndex("TenantId", "Id");

                    b.HasIndex("TenantId", "IntegrationId");

                    b.HasIndex("TenantId", "Status");

                    b.ToTable("brands");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Postgres.Entities.CampaignRows.CampaignRowEntity", b =>
                {
                    b.Property<long>("CampaignBoxId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ActivityHours")
                        .HasColumnType("text");

                    b.Property<long>("BrandId")
                        .HasColumnType("bigint");

                    b.Property<long>("CampaignId")
                        .HasColumnType("bigint");

                    b.Property<int>("CapType")
                        .HasColumnType("integer");

                    b.Property<string>("CountryCode")
                        .HasColumnType("text");

                    b.Property<long>("DailyCapValue")
                        .HasColumnType("bigint");

                    b.Property<bool>("EnableTraffic")
                        .HasColumnType("boolean");

                    b.Property<string>("Information")
                        .HasColumnType("text");

                    b.Property<int>("Priority")
                        .HasColumnType("integer");

                    b.Property<long>("Sequence")
                        .HasColumnType("bigint");

                    b.Property<int>("Weight")
                        .HasColumnType("integer");

                    b.HasKey("CampaignBoxId");

                    b.HasIndex("BrandId");

                    b.HasIndex("CampaignId");

                    b.HasIndex("CountryCode");

                    b.ToTable("campaign-rows");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Postgres.Entities.Campaigns.CampaignEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long>("Sequence")
                        .HasColumnType("bigint");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Id");

                    b.HasIndex("TenantId", "Name");

                    b.ToTable("campaigns");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Postgres.Entities.Integrations.IntegrationEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long>("Sequence")
                        .HasColumnType("bigint");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Id");

                    b.HasIndex("TenantId", "Name");

                    b.ToTable("integrations");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Postgres.Entities.Partners.AffiliateEntity", b =>
                {
                    b.Property<long>("AffiliateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("BankAccountNumber")
                        .HasColumnType("text");

                    b.Property<string>("BankAddress")
                        .HasColumnType("text");

                    b.Property<string>("BankBeneficiaryAddress")
                        .HasColumnType("text");

                    b.Property<string>("BankBeneficiaryName")
                        .HasColumnType("text");

                    b.Property<string>("BankIban")
                        .HasColumnType("text");

                    b.Property<string>("BankName")
                        .HasColumnType("text");

                    b.Property<string>("BankSwift")
                        .HasColumnType("text");

                    b.Property<string>("CompanyAddress")
                        .HasColumnType("text");

                    b.Property<string>("CompanyName")
                        .HasColumnType("text");

                    b.Property<string>("CompanyRegNumber")
                        .HasColumnType("text");

                    b.Property<string>("CompanyVatId")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("GeneralInfoApiKey")
                        .HasColumnType("text");

                    b.Property<int>("GeneralInfoCurrency")
                        .HasColumnType("integer");

                    b.Property<string>("GeneralInfoEmail")
                        .HasColumnType("text");

                    b.Property<string>("GeneralInfoPassword")
                        .HasColumnType("text");

                    b.Property<string>("GeneralInfoPhone")
                        .HasColumnType("text");

                    b.Property<int>("GeneralInfoRole")
                        .HasColumnType("integer");

                    b.Property<string>("GeneralInfoSkype")
                        .HasColumnType("text");

                    b.Property<int>("GeneralInfoState")
                        .HasColumnType("integer");

                    b.Property<string>("GeneralInfoUsername")
                        .HasColumnType("text");

                    b.Property<string>("GeneralInfoZipCode")
                        .HasColumnType("text");

                    b.Property<long>("Sequence")
                        .HasColumnType("bigint");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("AffiliateId");

                    b.HasIndex("TenantId", "AffiliateId");

                    b.HasIndex("TenantId", "CreatedAt");

                    b.HasIndex("TenantId", "GeneralInfoEmail")
                        .IsUnique();

                    b.HasIndex("TenantId", "GeneralInfoRole");

                    b.HasIndex("TenantId", "GeneralInfoUsername")
                        .IsUnique();

                    b.ToTable("affiliates");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Postgres.Entities.Brands.BrandEntity", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Postgres.Entities.Integrations.IntegrationEntity", "Integration")
                        .WithMany("Campaigns")
                        .HasForeignKey("IntegrationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("MarketingBox.Affiliate.Postgres.Entities.Brands.Payout", "Payout", b1 =>
                        {
                            b1.Property<long>("BrandEntityId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("bigint")
                                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");

                            b1.Property<int>("Currency")
                                .HasColumnType("integer");

                            b1.Property<int>("Plan")
                                .HasColumnType("integer");

                            b1.HasKey("BrandEntityId");

                            b1.ToTable("brands");

                            b1.WithOwner()
                                .HasForeignKey("BrandEntityId");
                        });

                    b.OwnsOne("MarketingBox.Affiliate.Postgres.Entities.Brands.Revenue", "Revenue", b1 =>
                        {
                            b1.Property<long>("BrandEntityId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("bigint")
                                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");

                            b1.Property<int>("Currency")
                                .HasColumnType("integer");

                            b1.Property<int>("Plan")
                                .HasColumnType("integer");

                            b1.HasKey("BrandEntityId");

                            b1.ToTable("brands");

                            b1.WithOwner()
                                .HasForeignKey("BrandEntityId");
                        });

                    b.Navigation("Integration");

                    b.Navigation("Payout");

                    b.Navigation("Revenue");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Postgres.Entities.CampaignRows.CampaignRowEntity", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Postgres.Entities.Brands.BrandEntity", "Brand")
                        .WithMany("CampaignBoxes")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MarketingBox.Affiliate.Postgres.Entities.Campaigns.CampaignEntity", "Campaign")
                        .WithMany("CampaignBoxes")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Campaign");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Postgres.Entities.Brands.BrandEntity", b =>
                {
                    b.Navigation("CampaignBoxes");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Postgres.Entities.Campaigns.CampaignEntity", b =>
                {
                    b.Navigation("CampaignBoxes");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Postgres.Entities.Integrations.IntegrationEntity", b =>
                {
                    b.Navigation("Campaigns");
                });
#pragma warning restore 612, 618
        }
    }
}
