using MarketingBox.Affiliate.Postgres.Entities.Boxes;
using MarketingBox.Affiliate.Postgres.Entities.Brands;
using MarketingBox.Affiliate.Postgres.Entities.CampaignBoxes;
using MarketingBox.Affiliate.Postgres.Entities.Campaigns;
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
        private const string CampaignTableName = "campaigns";
        private const string BoxTableName = "boxes";
        private const string CampaignBoxTableName = "campaign-boxes";
        private const string BrandTableName = "brands";

        public DbSet<PartnerEntity> Partners { get; set; }

        public DbSet<BoxEntity> Boxes { get; set; }

        public DbSet<BrandEntity> Brands { get; set; }

        public DbSet<CampaignEntity> Campaigns { get; set; }

        public DbSet<CampaignBoxEntity> CampaignBoxes { get; set; }



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
            SetBoxEntity(modelBuilder);
            SetBrandEntity(modelBuilder);
            SetCampaignEntity(modelBuilder);
            SetCampaignBoxEntity(modelBuilder);

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

        private void SetCampaignEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CampaignEntity>().ToTable(CampaignTableName);
            modelBuilder.Entity<CampaignEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<CampaignEntity>().OwnsOne(x => x.Payout);
            modelBuilder.Entity<CampaignEntity>().OwnsOne(x => x.Revenue);
            modelBuilder.Entity<CampaignEntity>()
                .HasOne(x => x.Brand)
                .WithMany(x => x.Campaigns)
                .HasForeignKey(x => x.BrandId);

            modelBuilder.Entity<CampaignEntity>().HasIndex(e => new { e.TenantId, e.Id });
            modelBuilder.Entity<CampaignEntity>().HasIndex(e => new { e.TenantId, e.BrandId });
            modelBuilder.Entity<CampaignEntity>().HasIndex(e => new { e.TenantId, e.Status });
        }

        private void SetCampaignBoxEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CampaignBoxEntity>().ToTable(CampaignBoxTableName);
            modelBuilder.Entity<CampaignBoxEntity>().HasKey(e => e.CampaignBoxId);
            modelBuilder.Entity<CampaignBoxEntity>()
                .HasOne(x => x.Box)
                .WithMany(x => x.CampaignBoxes)
                .HasForeignKey(x => x.BoxId);

            modelBuilder.Entity<CampaignBoxEntity>()
                .HasOne(x => x.Campaign)
                .WithMany(x => x.CampaignBoxes)
                .HasForeignKey(x => x.CampaignId);

            modelBuilder.Entity<CampaignBoxEntity>()
                .Property(e => e.ActivityHours)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v,
                        JsonSerializingSettings),
                    v =>
                        JsonConvert.DeserializeObject<ActivityHours[]>(v,
                            JsonSerializingSettings));

            modelBuilder.Entity<CampaignBoxEntity>().HasIndex(e => new { e.BoxId });
            modelBuilder.Entity<CampaignBoxEntity>().HasIndex(e => new { e.CampaignId });
            modelBuilder.Entity<CampaignBoxEntity>().HasIndex(e => new { e.CountryCode });
        }

        private void SetBrandEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrandEntity>().ToTable(BrandTableName);
            modelBuilder.Entity<BrandEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<BrandEntity>().HasIndex(e => new { e.TenantId, e.Id });
            modelBuilder.Entity<BrandEntity>().HasIndex(e => new { e.TenantId, e.Name });
        }

        private void SetBoxEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BoxEntity>().ToTable(BoxTableName);
            modelBuilder.Entity<BoxEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<BoxEntity>().HasIndex(e => new { e.TenantId, e.Id });
            modelBuilder.Entity<BoxEntity>().HasIndex(e => new { e.TenantId, e.Name });
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
