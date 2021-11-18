using MarketingBox.Affiliate.Postgres.Entities.AffiliateAccesses;
using MarketingBox.Affiliate.Postgres.Entities.Affiliates;
using MarketingBox.Affiliate.Postgres.Entities.Brands;
using MarketingBox.Affiliate.Postgres.Entities.CampaignRows;
using MarketingBox.Affiliate.Postgres.Entities.Campaigns;
using MarketingBox.Affiliate.Postgres.Entities.Integrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MarketingBox.Affiliate.Postgres
{
    public class DatabaseContext : DbContext
    {
        private static readonly JsonSerializerSettings JsonSerializingSettings =
            new() { NullValueHandling = NullValueHandling.Ignore };

        public const string Schema = "affiliate-service";

        private const string AffiliateTableName = "affiliates";
        private const string AffiliateAccessTableName = "affiliate_access";
        private const string BrandTableName = "brands";
        private const string CampaignTableName = "campaigns";
        private const string CampaignBoxTableName = "campaign-rows";
        private const string IntegrationTableName = "integrations";
        private const string AffiliateSubParamsTableName = "affiliatesubparam";

        public DbSet<AffiliateEntity> Affiliates { get; set; }
        public DbSet<AffiliateAccessEntity> AffiliateAccess { get; set; }

        public DbSet<CampaignEntity> Campaigns { get; set; }

        public DbSet<IntegrationEntity> Integrations { get; set; }

        public DbSet<BrandEntity> Brands { get; set; }

        public DbSet<CampaignRowEntity> CampaignRows { get; set; }
        public DbSet<AffiliateSubParamEntity> AffiliateSubParamCollection { get; set; }



        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
        public static ILoggerFactory LoggerFactory { get; set; }


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
            SetAffiliateAccessEntity(modelBuilder);
            SetCampaignEntity(modelBuilder);
            SetIntegrationEntity(modelBuilder);
            SetBrandEntity(modelBuilder);
            SetCampaignRowEntity(modelBuilder);
            SetAffiliateSubParamEntity(modelBuilder);

            base.OnModelCreating(modelBuilder);
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
                .HasOne(x => x.AccessIsGivenTo)
                .WithOne(x => x.Affiliate)
                .IsRequired(false);
        }

        private void SetAffiliateAccessEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AffiliateAccessEntity>().ToTable(AffiliateAccessTableName);
            modelBuilder.Entity<AffiliateAccessEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<AffiliateAccessEntity>().HasIndex(e => new { e.MasterAffiliateId, e.AffiliateId}).IsUnique();
            modelBuilder.Entity<AffiliateAccessEntity>().HasIndex(e => e.AffiliateId);
            modelBuilder.Entity<AffiliateAccessEntity>()
                .HasOne(e => e.MasterAffiliate)
                .WithOne(x => x.AccessIsGivenBy)
                .IsRequired(false);
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
            modelBuilder.Entity<BrandEntity>().HasIndex(e => new { e.TenantId, e.IntegrationId });
            modelBuilder.Entity<BrandEntity>().HasIndex(e => new { e.TenantId, e.Status });
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
            modelBuilder.Entity<CampaignRowEntity>().HasIndex(e => new { e.CountryCode });
        }

        private void SetIntegrationEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IntegrationEntity>().ToTable(IntegrationTableName);
            modelBuilder.Entity<IntegrationEntity>().HasKey(e => e.Id);
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

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
