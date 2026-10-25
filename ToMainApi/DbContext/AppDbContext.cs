using Microsoft.EntityFrameworkCore;
using ToMainApi.Models.Entities;

namespace ToMainApi.DbContext
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AgentProfile> AgentProfiles { get; set; }
        public DbSet<ModeratorProfile> ModeratorProfiles { get; set; }
        public DbSet<AdminProfile> AdminProfiles { get; set; }
        public DbSet<VehicleCategoryBySrts> VehicleCategoriesBySrts { get; set; }
        public DbSet<VehicleCategoryByOkp> VehicleCategoriesByOkp { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationPhoto> ApplicationPhotos { get; set; }
        public DbSet<Pto> Ptos { get; set; }
        public DbSet<PtoPricePolicy> PtoPolicies { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Prompt> Prompts { get; set; }
        public DbSet<PhotoUploadRequire> PhotoUploadRequires { get; set; }
        public DbSet<ApplicationProvidedDocument> ApplicationProvidedDocuments { get; set; }
        public DbSet<Notification> Notifications { get; set; } 
        public DbSet<RouteAINeuronNetwork> RouteAINeuronNetworks {get; set;}
        public DbSet<UserStatus> UserStatuses { get; set; }
        public DbSet<UserBlocks> UserBlocks { get; set; } 
        public DbSet<UserInvitation> UserInvitations { get; set; }
        public DbSet<PhotoInGallery> PhotoGallery { get; set; }
        public DbSet<AgentBranch> AgentBranches { get; set; }
        public DbSet<PtoExpert> PtoExperts { get; set; }
        public DbSet<PtoExpertCryptoKey> PtoExpertCryptoKeys { get; set; }
        public DbSet<PtoOperatorCryptoKey> PtoOperatorExpertCryptoKeys { get; set; }
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
