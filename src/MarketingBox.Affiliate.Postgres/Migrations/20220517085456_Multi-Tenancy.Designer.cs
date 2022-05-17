﻿// <auto-generated />
using System;
using System.Collections.Generic;
using MarketingBox.Affiliate.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketingBox.Affiliate.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20220517085456_Multi-Tenancy")]
    partial class MultiTenancy
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("affiliate-service")
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AffiliateAffiliatePayout", b =>
                {
                    b.Property<long>("AffiliatesId")
                        .HasColumnType("bigint");

                    b.Property<long>("PayoutsId")
                        .HasColumnType("bigint");

                    b.HasKey("AffiliatesId", "PayoutsId");

                    b.HasIndex("PayoutsId");

                    b.ToTable("AffiliateAffiliatePayout", "affiliate-service");
                });

            modelBuilder.Entity("BrandBrandPayout", b =>
                {
                    b.Property<long>("BrandsId")
                        .HasColumnType("bigint");

                    b.Property<long>("PayoutsId")
                        .HasColumnType("bigint");

                    b.HasKey("BrandsId", "PayoutsId");

                    b.HasIndex("PayoutsId");

                    b.ToTable("BrandBrandPayout", "affiliate-service");
                });

            modelBuilder.Entity("GeoOffer", b =>
                {
                    b.Property<int>("GeosId")
                        .HasColumnType("integer");

                    b.Property<long>("OffersId")
                        .HasColumnType("bigint");

                    b.HasKey("GeosId", "OffersId");

                    b.HasIndex("OffersId");

                    b.ToTable("GeoOffer", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.Affiliate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("ApiKey")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<int>("Currency")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("Skype")
                        .HasColumnType("text");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.Property<string>("ZipCode")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "CreatedAt");

                    b.HasIndex("TenantId", "Email")
                        .IsUnique();

                    b.HasIndex("TenantId", "Id");

                    b.HasIndex("TenantId", "Username")
                        .IsUnique();

                    b.ToTable("affiliates", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.AffiliatePayout", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Currency")
                        .HasColumnType("integer");

                    b.Property<int>("GeoId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("PayoutType")
                        .HasColumnType("integer");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("Currency");

                    b.HasIndex("GeoId");

                    b.HasIndex("ModifiedAt");

                    b.HasIndex("PayoutType");

                    b.HasIndex("TenantId", "Id");

                    b.ToTable("affiliate-payouts", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.AffiliateSubParam", b =>
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

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Id");

                    b.ToTable("affiliate-subparam", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.BrandBox.BrandBox", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<List<long>>("BrandIds")
                        .HasColumnType("bigint[]");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("TenantId", "Id");

                    b.HasIndex("TenantId", "Name")
                        .IsUnique();

                    b.ToTable("brandboxes", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Brands.Brand", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("IntegrationId")
                        .HasColumnType("bigint");

                    b.Property<int>("IntegrationType")
                        .HasColumnType("integer");

                    b.Property<string>("Link")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("IntegrationId");

                    b.HasIndex("IntegrationType");

                    b.HasIndex("TenantId", "Id");

                    b.ToTable("brands", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Brands.BrandPayout", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Currency")
                        .HasColumnType("integer");

                    b.Property<int>("GeoId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("PayoutType")
                        .HasColumnType("integer");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("Currency");

                    b.HasIndex("GeoId");

                    b.HasIndex("ModifiedAt");

                    b.HasIndex("PayoutType");

                    b.HasIndex("TenantId", "Id");

                    b.ToTable("brand-payouts", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.CampaignRows.CampaignRow", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

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

                    b.Property<int>("GeoId")
                        .HasColumnType("integer");

                    b.Property<string>("Information")
                        .HasColumnType("text");

                    b.Property<int>("Priority")
                        .HasColumnType("integer");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.Property<int>("Weight")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("CampaignId");

                    b.HasIndex("GeoId");

                    b.HasIndex("TenantId", "Id");

                    b.ToTable("campaign-rows", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Campaigns.Campaign", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

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

                    b.HasIndex("Alfa2Code");

                    b.HasIndex("Alfa3Code");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("Numeric");

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

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("TenantId", "Id");

                    b.HasIndex("TenantId", "Name")
                        .IsUnique();

                    b.ToTable("geos", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Integrations.Integration", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TenantId", "Id");

                    b.HasIndex("TenantId", "Name");

                    b.ToTable("integrations", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Languages.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code2")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Code3")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Code2");

                    b.HasIndex("Code3");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("languages", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates.OfferAffiliate", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AffiliateId")
                        .HasColumnType("bigint");

                    b.Property<long>("OfferId")
                        .HasColumnType("bigint");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.Property<string>("UniqueId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AffiliateId");

                    b.HasIndex("OfferId");

                    b.HasIndex("TenantId", "Id");

                    b.HasIndex("TenantId", "OfferId", "AffiliateId", "UniqueId")
                        .IsUnique();

                    b.ToTable("offer-affiliates", "affiliate-service");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Offers.Offer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("BrandId")
                        .HasColumnType("bigint");

                    b.Property<int>("Currency")
                        .HasColumnType("integer");

                    b.Property<int>("LanguageId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Privacy")
                        .HasColumnType("integer");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<string>("TenantId")
                        .HasColumnType("text");

                    b.Property<string>("UniqueId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("Currency");

                    b.HasIndex("LanguageId");

                    b.HasIndex("Name");

                    b.HasIndex("Privacy");

                    b.HasIndex("State");

                    b.HasIndex("TenantId", "Id");

                    b.ToTable("offers", "affiliate-service");
                });

            modelBuilder.Entity("AffiliateAffiliatePayout", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.Affiliate", null)
                        .WithMany()
                        .HasForeignKey("AffiliatesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.AffiliatePayout", null)
                        .WithMany()
                        .HasForeignKey("PayoutsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BrandBrandPayout", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Brands.Brand", null)
                        .WithMany()
                        .HasForeignKey("BrandsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Brands.BrandPayout", null)
                        .WithMany()
                        .HasForeignKey("PayoutsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GeoOffer", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Country.Geo", null)
                        .WithMany()
                        .HasForeignKey("GeosId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Offers.Offer", null)
                        .WithMany()
                        .HasForeignKey("OffersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.Affiliate", b =>
                {
                    b.OwnsOne("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.Bank", "Bank", b1 =>
                        {
                            b1.Property<long>("AffiliateId")
                                .HasColumnType("bigint");

                            b1.Property<string>("AccountNumber")
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)");

                            b1.Property<string>("Address")
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)");

                            b1.Property<string>("BeneficiaryAddress")
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)");

                            b1.Property<string>("BeneficiaryName")
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)");

                            b1.Property<string>("Iban")
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)");

                            b1.Property<string>("Name")
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)");

                            b1.Property<string>("Swift")
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)");

                            b1.HasKey("AffiliateId");

                            b1.ToTable("affiliates", "affiliate-service");

                            b1.WithOwner()
                                .HasForeignKey("AffiliateId");
                        });

                    b.OwnsOne("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.Company", "Company", b1 =>
                        {
                            b1.Property<long>("AffiliateId")
                                .HasColumnType("bigint");

                            b1.Property<string>("Address")
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)");

                            b1.Property<string>("Name")
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)");

                            b1.Property<string>("RegNumber")
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)");

                            b1.Property<string>("VatId")
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)");

                            b1.HasKey("AffiliateId");

                            b1.ToTable("affiliates", "affiliate-service");

                            b1.WithOwner()
                                .HasForeignKey("AffiliateId");
                        });

                    b.Navigation("Bank");

                    b.Navigation("Company");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.AffiliatePayout", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Country.Geo", "Geo")
                        .WithMany()
                        .HasForeignKey("GeoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Geo");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Brands.Brand", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Integrations.Integration", "Integration")
                        .WithMany("Brands")
                        .HasForeignKey("IntegrationId");

                    b.OwnsOne("MarketingBox.Affiliate.Service.Domain.Models.Brands.LinkParameters", "LinkParameters", b1 =>
                        {
                            b1.Property<long>("BrandId")
                                .HasColumnType("bigint");

                            b1.Property<string>("ClickId")
                                .IsRequired()
                                .HasMaxLength(20)
                                .HasColumnType("character varying(20)");

                            b1.Property<string>("Language")
                                .HasMaxLength(20)
                                .HasColumnType("character varying(20)");

                            b1.Property<string>("MPC_1")
                                .HasMaxLength(20)
                                .HasColumnType("character varying(20)");

                            b1.Property<string>("MPC_2")
                                .HasMaxLength(20)
                                .HasColumnType("character varying(20)");

                            b1.Property<string>("MPC_3")
                                .HasMaxLength(20)
                                .HasColumnType("character varying(20)");

                            b1.Property<string>("MPC_4")
                                .HasMaxLength(20)
                                .HasColumnType("character varying(20)");

                            b1.HasKey("BrandId");

                            b1.ToTable("brands", "affiliate-service");

                            b1.WithOwner()
                                .HasForeignKey("BrandId");
                        });

                    b.Navigation("Integration");

                    b.Navigation("LinkParameters");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Brands.BrandPayout", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Country.Geo", "Geo")
                        .WithMany()
                        .HasForeignKey("GeoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Geo");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.CampaignRows.CampaignRow", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Brands.Brand", "Brand")
                        .WithMany("CampaignRows")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Campaigns.Campaign", "Campaign")
                        .WithMany("CampaignRows")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Country.Geo", "Geo")
                        .WithMany()
                        .HasForeignKey("GeoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Campaign");

                    b.Navigation("Geo");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates.OfferAffiliate", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.Affiliate", "Affiliate")
                        .WithMany("OfferAffiliates")
                        .HasForeignKey("AffiliateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Offers.Offer", "Offer")
                        .WithMany("OfferAffiliates")
                        .HasForeignKey("OfferId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Affiliate");

                    b.Navigation("Offer");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Offers.Offer", b =>
                {
                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Brands.Brand", "Brand")
                        .WithMany("Offers")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MarketingBox.Affiliate.Service.Domain.Models.Languages.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Language");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Affiliates.Affiliate", b =>
                {
                    b.Navigation("OfferAffiliates");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Brands.Brand", b =>
                {
                    b.Navigation("CampaignRows");

                    b.Navigation("Offers");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Campaigns.Campaign", b =>
                {
                    b.Navigation("CampaignRows");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Integrations.Integration", b =>
                {
                    b.Navigation("Brands");
                });

            modelBuilder.Entity("MarketingBox.Affiliate.Service.Domain.Models.Offers.Offer", b =>
                {
                    b.Navigation("OfferAffiliates");
                });
#pragma warning restore 612, 618
        }
    }
}
