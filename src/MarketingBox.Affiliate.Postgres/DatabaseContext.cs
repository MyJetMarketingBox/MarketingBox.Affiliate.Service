using System.Diagnostics;
using MarketingBox.Affiliate.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;

namespace MarketingBox.Affiliate.Postgres
{
    public class DatabaseContext : DbContext
    {
        public const string Schema = "affiliate-service";

        private const string PartnerTableName = "partners";

        public DbSet<PartnerEntity> Partners { get; set; }

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
           
            base.OnModelCreating(modelBuilder);
        }

        private void SetPartnerEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PartnerEntity>().ToTable(PartnerTableName);
            modelBuilder.Entity<PartnerEntity>().HasKey(e => e.AffiliateId);
            modelBuilder.Entity<PartnerEntity>().OwnsOne(x => x.Bank);
            modelBuilder.Entity<PartnerEntity>().OwnsOne(x => x.Company);
            modelBuilder.Entity<PartnerEntity>().OwnsOne(x => x.GeneralInfo);
            modelBuilder.Entity<PartnerEntity>().HasIndex(e => new {e.TenantId, e.AffiliateId});
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
