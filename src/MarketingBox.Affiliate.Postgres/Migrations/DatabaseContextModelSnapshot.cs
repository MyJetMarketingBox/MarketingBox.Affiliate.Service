﻿// <auto-generated />
using System;
using MarketingBox.Affiliate.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("affiliate-service")
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.AffiliateAccesses.AffiliateAccessEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AffiliateId")
                        .HasColumnType("bigint");

                    b.Property<long>("MasterAffiliateId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AffiliateId");

                    b.HasIndex("MasterAffiliateId", "AffiliateId")
                        .IsUnique();

                    b.ToTable("affiliate_access", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.AffiliateEntity", b =>
                {
                    b.Property<long>("AffiliateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("AffiliateId"));

                    b.Property<long>("AccessIsGivenById")
                        .HasColumnType("bigint");

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

                    b.Property<string>("LandingUrl")
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

                    b.ToTable("affiliates", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.AffiliateSubParamEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AffiliateId")
                        .HasColumnType("bigint");

                    b.Property<string>("ParamName")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("ParamValue")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.HasKey("Id");

                    b.ToTable("affiliatesubparam", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Brands.BrandEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

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

                    b.HasIndex("TenantId", "Status");

                    b.ToTable("brands", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.CampaignRows.CampaignRowEntity", b =>
                {
                    b.Property<long>("CampaignBoxId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("CampaignBoxId"));

                    b.Property<string>("ActivityHours")
                        .HasColumnType("text");

                    b.Property<long>("BrandId")
                        .HasColumnType("bigint");

                    b.Property<long>("CampaignId")
                        .HasColumnType("bigint");

                    b.Property<int>("CapType")
                        .HasColumnType("integer");

                    b.Property<long>("DailyCapValue")
                        .HasColumnType("bigint");

                    b.Property<bool>("EnableTraffic")
                        .HasColumnType("boolean");

                    b.Property<int?>("GeoId")
                        .HasColumnType("integer");

                    b.Property<string>("Information")
                        .HasColumnType("text");

                    b.Property<int>("Priority")
                        .HasColumnType("integer");

                    b.Property<int>("Weight")
                        .HasColumnType("integer");

                    b.HasKey("CampaignBoxId");

                    b.HasIndex("BrandId");

                    b.HasIndex("CampaignId");

                    b.HasIndex("GeoId");

                    b.ToTable("campaign-rows", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Campaigns.CampaignEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long>("Sequence")
                        .HasColumnType("bigint");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Id");

                    b.HasIndex("TenantId", "Name");

                    b.ToTable("campaigns", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Country.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Alfa2Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Alfa3Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Numeric")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("countries", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Country.Geo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int[]>("CountryIds")
                        .HasColumnType("integer[]");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("geos", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Integrations.IntegrationEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("AffiliateId")
                        .HasColumnType("bigint");

                    b.Property<int>("IntegrationType")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long?>("OfferId")
                        .HasColumnType("bigint");

                    b.Property<long>("Sequence")
                        .HasColumnType("bigint");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AffiliateId");

                    b.HasIndex("IntegrationType");

                    b.HasIndex("OfferId");

                    b.HasIndex("TenantId", "Id");

                    b.HasIndex("TenantId", "Name");

                    b.ToTable("integrations", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Offers.Offer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("BrnadId")
                        .HasColumnType("bigint");

                    b.Property<string>("OfferLink")
                        .HasColumnType("text");

                    b.Property<string>("OfferName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BrnadId");

                    b.ToTable("offers", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Offers.OfferSubParameter", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("OfferId")
                        .HasColumnType("bigint");

                    b.Property<string>("ParamName")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("ParamValue")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.HasKey("Id");

                    b.HasIndex("OfferId");

                    b.ToTable("offersubparams", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Brands.BrandEntity", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Integrations.IntegrationEntity", "Integration")
                        .WithMany("Campaigns")
                        .HasForeignKey("IntegrationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("MarketingBox.Affiliate.Service.Domain.Models.Brands.Payout", "Payout", b1 =>
                        {
                            b1.Property<long>("BrandEntityId")
                                .HasColumnType("bigint");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");

                            b1.Property<int>("Currency")
                                .HasColumnType("integer");

                            b1.Property<int>("Plan")
                                .HasColumnType("integer");

                            b1.HasKey("BrandEntityId");

                            b1.ToTable("brands", "affiliate-service");

                            b1.WithOwner()
                                .HasForeignKey("BrandEntityId");
                        });

                    b.OwnsOne("MarketingBox.Affiliate.Service.Domain.Models.Brands.Revenue", "Revenue", b1 =>
                        {
                            b1.Property<long>("BrandEntityId")
                                .HasColumnType("bigint");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("numeric");

                            b1.Property<int>("Currency")
                                .HasColumnType("integer");

                            b1.Property<int>("Plan")
                                .HasColumnType("integer");

                            b1.HasKey("BrandEntityId");

                            b1.ToTable("brands", "affiliate-service");

                            b1.WithOwner()
                                .HasForeignKey("BrandEntityId");
                        });

                    b.Navigation("Integration");

                    b.Navigation("Payout");

                    b.Navigation("Revenue");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.CampaignRows.CampaignRowEntity", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Brands.BrandEntity", "Brand")
                        .WithMany("CampaignBoxes")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Campaigns.CampaignEntity", "Campaign")
                        .WithMany("CampaignBoxes")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Country.Geo", "Geo")
                        .WithMany()
                        .HasForeignKey("GeoId");

                    b.Navigation("Brand");

                    b.Navigation("Campaign");

                    b.Navigation("Geo");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Integrations.IntegrationEntity", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.AffiliateEntity", null)
                        .WithMany("Integrations")
                        .HasForeignKey("AffiliateId");

                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Offers.Offer", null)
                        .WithMany("Integrations")
                        .HasForeignKey("OfferId");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Offers.Offer", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Brands.BrandEntity", null)
                        .WithMany("Offers")
                        .HasForeignKey("BrnadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Offers.OfferSubParameter", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Offers.Offer", null)
                        .WithMany("Parameters")
                        .HasForeignKey("OfferId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.AffiliateEntity", b =>
                {
                    b.Navigation("Integrations");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Brands.BrandEntity", b =>
                {
                    b.Navigation("CampaignBoxes");

                    b.Navigation("Offers");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Campaigns.CampaignEntity", b =>
                {
                    b.Navigation("CampaignBoxes");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Integrations.IntegrationEntity", b =>
                {
                    b.Navigation("Campaigns");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Offers.Offer", b =>
                {
                    b.Navigation("Integrations");

                    b.Navigation("Parameters");
                });
#pragma warning restore 612, 618
        }
    }
}
