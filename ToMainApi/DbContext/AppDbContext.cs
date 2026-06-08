using Microsoft.EntityFrameworkCore;
using ToMainApi.Models.Entities;

namespace ToMainApi.DbContext
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AgentProfile> AgentProfiles { get; set; }
        public DbSet<ModeratorProfile> ModeratorProfiles { get; set; }
        public DbSet<AdminProfile> AdminProfiles { get; set; }
        public DbSet<VehicleCategory> VehicleCategories { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public DbSet<ApplicationPhoto> ApplicationPhotos { get; set; }
        public DbSet<Pto> Ptos { get; set; }
        public DbSet<PtoPricePolicy> PtoPolicies { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Prompt> Prompts { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
