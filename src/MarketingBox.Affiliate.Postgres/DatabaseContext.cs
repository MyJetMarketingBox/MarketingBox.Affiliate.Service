using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyJetWallet.Sdk.Postgres;
using Newtonsoft.Json;

namespace MarketingBox.Affiliate.Postgres;

public class DatabaseContext : MyDbContext
{
    public const string Schema = "affiliate-service";

    private const string AffiliateTableName = "affiliates";
    private const string BrandTableName = "brands";
    private const string CampaignTableName = "campaigns";
    private const string CampaignRowTableName = "campaign-rows";
    private const string IntegrationTableName = "integrations";
    private const string AffiliateSubParamsTableName = "affiliate-subparam";
    private const string OfferTableName = "offers";
    private const string OfferSubParamsTableName = "offer-subparams";
    private const string GeoTableName = "geos";
    private const string CountryTableName = "countries";
    private const string BankTableName = "banks";
    private const string CompanyTableName = "companies";
    private const string AffiliatePayoutTableName = "affiliate-payouts";
    private const string BrandPayoutTableName = "brand-payouts";
    private const string OfferAffiliatesTableName = "offer-affiliates";

    private static readonly JsonSerializerSettings JsonSerializingSettings =
        new() {NullValueHandling = NullValueHandling.Ignore};


    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Service.Domain.Models.Affiliates.Affiliate> Affiliates { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Integration> Integrations { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<CampaignRow> CampaignRows { get; set; }
    public DbSet<AffiliateSubParam> AffiliateSubParams { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Geo> Geos { get; set; }
    public DbSet<OfferSubParameter> SubParameters { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<AffiliatePayout> AffiliatePayouts { get; set; }
    public DbSet<BrandPayout> BrandPayouts { get; set; }
    public DbSet<OfferAffiliates> OfferAffiliates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (LoggerFactory != null) optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        SetBank(modelBuilder);
        SetCompany(modelBuilder);
        SetAffiliate(modelBuilder);
        SetCampaign(modelBuilder);
        SetIntegration(modelBuilder);
        SetBrand(modelBuilder);
        SetCampaignRow(modelBuilder);
        SetAffiliateSubParam(modelBuilder);
        SetOffer(modelBuilder);
        SetOfferSubParam(modelBuilder);
        SetCountry(modelBuilder);
        SetGeo(modelBuilder);
        SetBrandPayout(modelBuilder);
        SetAffiliatePayout(modelBuilder);
        SetOfferAffiliate(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void SetOfferAffiliate(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OfferAffiliates>().ToTable(OfferAffiliatesTableName);
        modelBuilder.Entity<OfferAffiliates>().HasKey(x => x.Id);
        modelBuilder.Entity<OfferAffiliates>()
            .HasOne(x => x.Affiliate)
            .WithMany(x => x.OfferAffiliates)
            .HasForeignKey(x => x.AffiliateId);
        modelBuilder.Entity<OfferAffiliates>()
            .HasOne(x => x.Offer)
            .WithMany(x => x.OfferAffiliates)
            .HasForeignKey(x => x.OfferId);
    }

    private static void SetAffiliatePayout(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AffiliatePayout>().ToTable(AffiliatePayoutTableName);
        modelBuilder.Entity<AffiliatePayout>().HasKey(x => x.Id);
        modelBuilder.Entity<AffiliatePayout>().HasIndex(x => x.CreatedAt);
        modelBuilder.Entity<AffiliatePayout>().HasIndex(x => x.ModifiedAt);
        modelBuilder.Entity<AffiliatePayout>().HasIndex(x => x.PayoutType);
        modelBuilder.Entity<AffiliatePayout>().HasIndex(x => x.Currency);

        modelBuilder.Entity<AffiliatePayout>()
            .HasOne(x => x.Geo)
            .WithMany()
            .HasForeignKey(x => x.GeoId);
    }

    private static void SetBrandPayout(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BrandPayout>().ToTable(BrandPayoutTableName);
        modelBuilder.Entity<BrandPayout>().HasKey(x => x.Id);
        modelBuilder.Entity<BrandPayout>().HasIndex(x => x.CreatedAt);
        modelBuilder.Entity<BrandPayout>().HasIndex(x => x.ModifiedAt);
        modelBuilder.Entity<BrandPayout>().HasIndex(x => x.PayoutType);
        modelBuilder.Entity<BrandPayout>().HasIndex(x => x.Currency);

        modelBuilder.Entity<BrandPayout>()
            .HasOne(x => x.Geo)
            .WithMany()
            .HasForeignKey(x => x.GeoId);
    }

    private static void SetBank(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bank>().ToTable(BankTableName);
        modelBuilder.Entity<Bank>().HasKey(x => x.Id);
        modelBuilder.Entity<Bank>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Bank>().Property(x => x.Name).IsRequired();
    }

    private static void SetCompany(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>().ToTable(CompanyTableName);
        modelBuilder.Entity<Company>().HasKey(x => x.Id);
        modelBuilder.Entity<Company>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Company>().Property(x => x.Name).IsRequired();
    }

    private static void SetGeo(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Geo>().ToTable(GeoTableName);
        modelBuilder.Entity<Geo>().HasKey(x => x.Id);
        modelBuilder.Entity<Geo>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Geo>().Property(x => x.Name).IsRequired();
        modelBuilder.Entity<Geo>().HasIndex(x => x.CreatedAt);
    }

    private static void SetCountry(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>().ToTable(CountryTableName);
        modelBuilder.Entity<Country>().HasKey(x => x.Id);
        modelBuilder.Entity<Country>().Property(x => x.Name).IsRequired();
        modelBuilder.Entity<Country>().Property(x => x.Alfa2Code).IsRequired();
        modelBuilder.Entity<Country>().Property(x => x.Alfa3Code).IsRequired();
        modelBuilder.Entity<Country>().Property(x => x.Numeric).IsRequired();

        modelBuilder.Entity<Country>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Country>().HasIndex(x => x.Alfa2Code);
        modelBuilder.Entity<Country>().HasIndex(x => x.Alfa3Code);
        modelBuilder.Entity<Country>().HasIndex(x => x.Numeric);
    }

    private static void SetOffer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Offer>().ToTable(OfferTableName);
        modelBuilder.Entity<Offer>().HasKey(x => x.Id);
        modelBuilder.Entity<Offer>()
            .HasMany(x => x.Parameters)
            .WithOne()
            .HasForeignKey(x => x.OfferId);
    }

    private static void SetOfferSubParam(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OfferSubParameter>().ToTable(OfferSubParamsTableName);

        modelBuilder.Entity<OfferSubParameter>().Property(e => e.Id).UseIdentityColumn();
        modelBuilder.Entity<OfferSubParameter>().HasKey(e => e.Id);

        modelBuilder.Entity<OfferSubParameter>().Property(e => e.ParamName).HasMaxLength(64);
        modelBuilder.Entity<OfferSubParameter>().Property(e => e.ParamValue).HasMaxLength(512);
    }

    private static void SetAffiliateSubParam(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AffiliateSubParam>().ToTable(AffiliateSubParamsTableName);

        modelBuilder.Entity<AffiliateSubParam>().Property(e => e.Id).UseIdentityColumn();
        modelBuilder.Entity<AffiliateSubParam>().HasKey(e => e.Id);

        modelBuilder.Entity<AffiliateSubParam>().Property(e => e.ParamName).HasMaxLength(64);
        modelBuilder.Entity<AffiliateSubParam>().Property(e => e.ParamValue).HasMaxLength(512);
    }

    private static void SetAffiliate(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Service.Domain.Models.Affiliates.Affiliate>().ToTable(AffiliateTableName);
        modelBuilder.Entity<Service.Domain.Models.Affiliates.Affiliate>().HasKey(e => e.Id);

        modelBuilder.Entity<Service.Domain.Models.Affiliates.Affiliate>()
            .HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId);
        modelBuilder.Entity<Service.Domain.Models.Affiliates.Affiliate>()
            .HasOne(x => x.Bank)
            .WithMany()
            .HasForeignKey(x => x.BankId);

        modelBuilder.Entity<Service.Domain.Models.Affiliates.Affiliate>()
            .HasIndex(e => new {e.TenantId, e.Id});
        modelBuilder.Entity<Service.Domain.Models.Affiliates.Affiliate>()
            .HasIndex(e => new {e.TenantId, e.Email}).IsUnique();
        modelBuilder.Entity<Service.Domain.Models.Affiliates.Affiliate>()
            .HasIndex(e => new {e.TenantId, e.Username}).IsUnique();
        modelBuilder.Entity<Service.Domain.Models.Affiliates.Affiliate>()
            .HasIndex(e => new {e.TenantId, e.CreatedAt});
    }

    private static void SetBrand(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>().ToTable(BrandTableName);
        modelBuilder.Entity<Brand>().HasKey(e => e.Id);

        modelBuilder.Entity<Brand>()
            .HasOne(x => x.Integration)
            .WithMany(x => x.Brands)
            .HasForeignKey(x => x.IntegrationId);

        modelBuilder.Entity<Brand>().HasIndex(e => new {e.TenantId, e.Id});
        modelBuilder.Entity<Brand>().HasIndex(e => new {e.TenantId, e.Status});
    }

    private static void SetCampaignRow(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CampaignRow>().ToTable(CampaignRowTableName);
        modelBuilder.Entity<CampaignRow>().HasKey(e => e.Id);
        modelBuilder.Entity<CampaignRow>()
            .HasOne(x => x.Campaign)
            .WithMany(x => x.CampaignRows)
            .HasForeignKey(x => x.CampaignId);
        modelBuilder.Entity<CampaignRow>()
            .HasOne(x => x.Brand)
            .WithMany(x => x.CampaignRows)
            .HasForeignKey(x => x.BrandId);
        modelBuilder.Entity<CampaignRow>()
            .HasOne(x => x.Geo)
            .WithMany()
            .HasForeignKey(x => x.GeoId);

        modelBuilder.Entity<CampaignRow>()
            .Property(e => e.ActivityHours)
            .HasConversion(
                v => JsonConvert.SerializeObject(v,
                    JsonSerializingSettings),
                v =>
                    JsonConvert.DeserializeObject<ActivityHours[]>(v,
                        JsonSerializingSettings));

        modelBuilder.Entity<CampaignRow>().HasIndex(e => new {e.CampaignId});
        modelBuilder.Entity<CampaignRow>().HasIndex(e => new {e.BrandId});
    }

    private static void SetIntegration(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Integration>().ToTable(IntegrationTableName);
        modelBuilder.Entity<Integration>().HasKey(e => e.Id);

        modelBuilder.Entity<Integration>().Property(e => e.Id);

        modelBuilder.Entity<Integration>().HasIndex(e => new {e.TenantId, e.Id});
        modelBuilder.Entity<Integration>().HasIndex(e => new {e.TenantId, e.Name});
    }

    private static void SetCampaign(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Campaign>().ToTable(CampaignTableName);
        modelBuilder.Entity<Campaign>().HasKey(e => e.Id);
        modelBuilder.Entity<Campaign>().HasIndex(e => new {e.TenantId, e.Id});
        modelBuilder.Entity<Campaign>().HasIndex(e => new {e.TenantId, e.Name});
        modelBuilder.Entity<Campaign>()
            .HasMany(e => e.OfferAffiliates)
            .WithOne(x => x.Campaign)
            .HasForeignKey(x => x.CampaignId);
    }

    public async Task AddNewAffiliateSubParam(IEnumerable<AffiliateSubParam> subParams)
    {
        await AffiliateSubParams.AddRangeAsync(subParams);
        await SaveChangesAsync();
    }
}