using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Brands;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Campaigns;
using MarketingBox.Affiliate.Service.Domain.Models.Country;
using MarketingBox.Affiliate.Service.Domain.Models.Integrations;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Postgres;
using Newtonsoft.Json;

namespace MarketingBox.Affiliate.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        private static readonly JsonSerializerSettings JsonSerializingSettings =
            new() { NullValueHandling = NullValueHandling.Ignore };

        public const string Schema = "affiliate-service";

        private const string AffiliateTableName = "affiliates";
        private const string BrandTableName = "brands";
        private const string CampaignTableName = "campaigns";
        private const string CampaignBoxTableName = "campaign-rows";
        private const string IntegrationTableName = "integrations";
        private const string AffiliateSubParamsTableName = "affiliatesubparam";
        private const string OfferTableName = "offers";
        private const string OfferSubParamsTableName = "offersubparams";
        private const string GeoTableName = "geos";
        private const string CountryTableName = "countries";

        public DbSet<AffiliateEntity> Affiliates { get; set; }

        public DbSet<CampaignEntity> Campaigns { get; set; }

        public DbSet<IntegrationEntity> Integrations { get; set; }

        public DbSet<BrandEntity> Brands { get; set; }

        public DbSet<CampaignRowEntity> CampaignRows { get; set; }
        public DbSet<AffiliateSubParamEntity> AffiliateSubParamCollection { get; set; }

        public DbSet<Offer> Offers { get; set; }
        public DbSet<OfferSubParameter> SubParameters { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Geo> Geos { get; set; }
        
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            SetAffiliateEntity(modelBuilder);
            SetCampaignEntity(modelBuilder);
            SetIntegrationEntity(modelBuilder);
            SetBrandEntity(modelBuilder);
            SetCampaignRowEntity(modelBuilder);
            SetAffiliateSubParamEntity(modelBuilder);
            SetOffer(modelBuilder);
            SetSubParam(modelBuilder);
            SetCountry(modelBuilder);
            SetCountryBox(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetCountryBox(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Geo>().ToTable(GeoTableName);
            modelBuilder.Entity<Geo>().HasKey(x => x.Id);
            modelBuilder.Entity<Geo>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Geo>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<Geo>().HasIndex(x => x.CreatedAt);
            modelBuilder.Entity<Geo>()
                .HasMany<CampaignRowEntity>()
                .WithOne(x => x.Geo)
                .HasForeignKey(x => x.GeoId);
        }

        private void SetCountry(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().ToTable(CountryTableName);
            modelBuilder.Entity<Country>().HasKey(x => x.Id);
            modelBuilder.Entity<Country>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Country>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<Country>().Property(x => x.Alfa2Code).IsRequired();
            modelBuilder.Entity<Country>().Property(x => x.Alfa3Code).IsRequired();
            modelBuilder.Entity<Country>().Property(x => x.Numeric).IsRequired();
        }

        private void SetOffer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Offer>().ToTable(OfferTableName);
            modelBuilder.Entity<Offer>().HasKey(x => x.Id);
            modelBuilder.Entity<Offer>()
                .HasMany(x => x.Parameters)
                .WithOne()
                .HasForeignKey(x => x.OfferId);
            modelBuilder.Entity<Offer>()
                .HasMany(x => x.Integrations)
                .WithOne()
                .HasForeignKey(x => x.OfferId);
        }

        private void SetSubParam(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OfferSubParameter>().ToTable(OfferSubParamsTableName);
            
            modelBuilder.Entity<OfferSubParameter>().Property(e => e.Id).UseIdentityColumn();
            modelBuilder.Entity<OfferSubParameter>().HasKey(e => e.Id);
            
            modelBuilder.Entity<OfferSubParameter>().Property(e => e.ParamName).HasMaxLength(64);
            modelBuilder.Entity<OfferSubParameter>().Property(e => e.ParamValue).HasMaxLength(512);
        }

        private void SetAffiliateSubParamEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AffiliateSubParamEntity>().ToTable(AffiliateSubParamsTableName);
            
            modelBuilder.Entity<AffiliateSubParamEntity>().Property(e => e.Id).UseIdentityColumn();
            modelBuilder.Entity<AffiliateSubParamEntity>().HasKey(e => e.Id);
            
            modelBuilder.Entity<AffiliateSubParamEntity>().Property(e => e.ParamName).HasMaxLength(64);
            modelBuilder.Entity<AffiliateSubParamEntity>().Property(e => e.ParamValue).HasMaxLength(512);
        }

        private void SetAffiliateEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AffiliateEntity>().ToTable(AffiliateTableName);
            modelBuilder.Entity<AffiliateEntity>().HasKey(e => e.AffiliateId);
            modelBuilder.Entity<AffiliateEntity>().HasIndex(e => new { e.TenantId, e.AffiliateId});
            modelBuilder.Entity<AffiliateEntity>().HasIndex(e => new { e.TenantId, e.GeneralInfoEmail }).IsUnique(true);
            modelBuilder.Entity<AffiliateEntity>().HasIndex(e => new { e.TenantId, e.GeneralInfoUsername }).IsUnique(true);
            modelBuilder.Entity<AffiliateEntity>().HasIndex(e => new { e.TenantId, e.CreatedAt });
            modelBuilder.Entity<AffiliateEntity>().HasIndex(e => new { e.TenantId, e.GeneralInfoRole });

            modelBuilder.Entity<AffiliateEntity>()
                .HasMany(e => e.Integrations)
                .WithOne()
                .HasForeignKey(e => e.AffiliateId);
        }

        private void SetBrandEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrandEntity>().ToTable(BrandTableName);
            modelBuilder.Entity<BrandEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<BrandEntity>().OwnsOne(x => x.Payout);
            modelBuilder.Entity<BrandEntity>().OwnsOne(x => x.Revenue);
            modelBuilder.Entity<BrandEntity>()
                .HasOne(x => x.Integration)
                .WithMany(x => x.Campaigns)
                .HasForeignKey(x => x.IntegrationId);

            modelBuilder.Entity<BrandEntity>().HasIndex(e => new { e.TenantId, e.Id });
            modelBuilder.Entity<BrandEntity>().HasIndex(e => new { e.TenantId, e.Status });
            
            modelBuilder.Entity<BrandEntity>()
                .HasMany(e => e.Offers)
                .WithOne()
                .HasForeignKey(e => e.BrnadId);
        }

        private void SetCampaignRowEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CampaignRowEntity>().ToTable(CampaignBoxTableName);
            modelBuilder.Entity<CampaignRowEntity>().HasKey(e => e.CampaignBoxId);
            modelBuilder.Entity<CampaignRowEntity>()
                .HasOne(x => x.Campaign)
                .WithMany(x => x.CampaignBoxes)
                .HasForeignKey(x => x.CampaignId);
            modelBuilder.Entity<CampaignRowEntity>()
                .HasOne(x => x.Brand)
                .WithMany(x => x.CampaignBoxes)
                .HasForeignKey(x => x.BrandId);

            modelBuilder.Entity<CampaignRowEntity>()
                .Property(e => e.ActivityHours)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v,
                        JsonSerializingSettings),
                    v =>
                        JsonConvert.DeserializeObject<ActivityHours[]>(v,
                            JsonSerializingSettings));

            modelBuilder.Entity<CampaignRowEntity>().HasIndex(e => new {e.CampaignId});
            modelBuilder.Entity<CampaignRowEntity>().HasIndex(e => new {e.BrandId});
        }

        private void SetIntegrationEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IntegrationEntity>().ToTable(IntegrationTableName);
            modelBuilder.Entity<IntegrationEntity>().HasKey(e => e.Id);
            
            modelBuilder.Entity<IntegrationEntity>().Property(e => e.Id);
            modelBuilder.Entity<IntegrationEntity>().Property(e => e.Sequence);
            
            modelBuilder.Entity<IntegrationEntity>().HasIndex(e => e.IntegrationType);
            modelBuilder.Entity<IntegrationEntity>().HasIndex(e => new { e.TenantId, e.Id });
            modelBuilder.Entity<IntegrationEntity>().HasIndex(e => new { e.TenantId, e.Name });
        }

        private void SetCampaignEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CampaignEntity>().ToTable(CampaignTableName);
            modelBuilder.Entity<CampaignEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<CampaignEntity>().HasIndex(e => new { e.TenantId, e.Id });
            modelBuilder.Entity<CampaignEntity>().HasIndex(e => new { e.TenantId, e.Name });
        }

        public async Task AddNewAffiliateSubParam(IEnumerable<AffiliateSubParamEntity> subParams)
        {
            await AffiliateSubParamCollection.AddRangeAsync(subParams);
            await SaveChangesAsync();
        }
    }
}