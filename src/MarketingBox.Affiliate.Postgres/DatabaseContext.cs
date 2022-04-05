using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Affiliate.Service.Domain.Models.Languages;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using Microsoft.EntityFrameworkCore;
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
    private const string GeoTableName = "geos";
    private const string CountryTableName = "countries";
    private const string LanguageTableName = "languages";
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
    public DbSet<Language> Languages { get; set; }
    public DbSet<Geo> Geos { get; set; }
    public DbSet<AffiliatePayout> AffiliatePayouts { get; set; }
    public DbSet<BrandPayout> BrandPayouts { get; set; }
    public DbSet<OfferAffiliate> OfferAffiliates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (LoggerFactory != null) optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        SetAffiliate(modelBuilder);
        SetCampaign(modelBuilder);
        SetIntegration(modelBuilder);
        SetBrand(modelBuilder);
        SetCampaignRow(modelBuilder);
        SetAffiliateSubParam(modelBuilder);
        SetOffer(modelBuilder);
        SetCountry(modelBuilder);
        SetLanguage(modelBuilder);
        SetGeo(modelBuilder);
        SetBrandPayout(modelBuilder);
        SetAffiliatePayout(modelBuilder);
        SetOfferAffiliate(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void SetOfferAffiliate(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OfferAffiliate>().ToTable(OfferAffiliatesTableName);
        modelBuilder.Entity<OfferAffiliate>().HasKey(x => x.Id);
        modelBuilder.Entity<OfferAffiliate>()
            .HasOne(x => x.Affiliate)
            .WithMany(x => x.OfferAffiliates)
            .HasForeignKey(x => x.AffiliateId);
        modelBuilder.Entity<OfferAffiliate>()
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

    private static void SetLanguage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Language>().ToTable(LanguageTableName);
        modelBuilder.Entity<Language>().HasKey(x => x.Id);
        modelBuilder.Entity<Language>().Property(x => x.Name).IsRequired();
        modelBuilder.Entity<Language>().Property(x => x.Code2).IsRequired();
        modelBuilder.Entity<Language>().Property(x => x.Code3).IsRequired();

        modelBuilder.Entity<Language>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Language>().HasIndex(x => x.Code2);
        modelBuilder.Entity<Language>().HasIndex(x => x.Code3);
    }

    private static void SetOffer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Offer>().ToTable(OfferTableName);
        modelBuilder.Entity<Offer>().HasKey(x => x.Id);
        modelBuilder.Entity<Offer>()
            .HasOne(x => x.Language)
            .WithMany()
            .HasForeignKey(x => x.LanguageId);
        modelBuilder.Entity<Offer>()
            .HasOne(x => x.Brand)
            .WithMany(x => x.Offers)
            .HasForeignKey(x => x.BrandId);
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
            .OwnsOne(e => e.Bank);
        modelBuilder.Entity<Service.Domain.Models.Affiliates.Affiliate>()
            .OwnsOne(e => e.Company);

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
            .OwnsOne(e => e.LinkParameters);

        modelBuilder.Entity<Brand>()
            .HasOne(x => x.Integration)
            .WithMany(x => x.Brands)
            .HasForeignKey(x => x.IntegrationId);

        modelBuilder.Entity<Brand>().HasIndex(e => new {e.TenantId, e.Id});
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
                    JsonConvert.DeserializeObject<List<ActivityHours>>(v,
                        JsonSerializingSettings));

        modelBuilder.Entity<CampaignRow>().HasIndex(e => e.CampaignId);
        modelBuilder.Entity<CampaignRow>().HasIndex(e => e.BrandId);
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