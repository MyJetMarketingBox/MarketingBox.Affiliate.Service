using MarketingBox.Affiliate.Postgres.Entities.Brands;
using MarketingBox.Affiliate.Postgres.Entities.CampaignRows;
using MarketingBox.Affiliate.Postgres.Entities.Campaigns;
using MarketingBox.Affiliate.Postgres.Entities.Integrations;
using MarketingBox.Affiliate.Postgres.Entities.Partners;
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

        private const string PartnerTableName = "partners";
        private const string BrandTableName = "brands";
        private const string CampaignTableName = "campaigns";
        private const string CampaignBoxTableName = "campaign-rows";
        private const string IntegrationTableName = "integrations";

        public DbSet<PartnerEntity> Partners { get; set; }

        public DbSet<CampaignEntity> Campaigns { get; set; }

        public DbSet<IntegrationEntity> Integrations { get; set; }

        public DbSet<BrandEntity> Brands { get; set; }

        public DbSet<CampaignRowEntity> CampaignRows { get; set; }



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

            SetPartnerEntity(modelBuilder);
            SetCampaignEntity(modelBuilder);
            SetIntegrationEntity(modelBuilder);
            SetBrandEntity(modelBuilder);
            SetCampaignRowEntity(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetPartnerEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PartnerEntity>().ToTable(PartnerTableName);
            modelBuilder.Entity<PartnerEntity>().HasKey(e => e.AffiliateId);
            modelBuilder.Entity<PartnerEntity>().OwnsOne(x => x.Bank);
            modelBuilder.Entity<PartnerEntity>().OwnsOne(x => x.Company);
            modelBuilder.Entity<PartnerEntity>().OwnsOne(x => x.GeneralInfo);
            modelBuilder.Entity<PartnerEntity>().HasIndex(e => new { e.TenantId, e.AffiliateId});
            //TODO: This IS NOT SUPPORTED BY EF BUT IT IS WRITTEN IN MIGRATION
            // modelBuilder.Entity<PartnerEntity>().HasIndex(e => new { e.TenantId, e.GeneralInfo.Email }).IsUnique(true);
            // modelBuilder.Entity<PartnerEntity>().HasIndex(e => new { e.TenantId, e.GeneralInfo.Username }).IsUnique(true);
            // modelBuilder.Entity<PartnerEntity>().HasIndex(e => new { e.TenantId, e.GeneralInfo.CreatedAt });
            // modelBuilder.Entity<PartnerEntity>().HasIndex(e => new { e.TenantId, e.GeneralInfo.Role });
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
