using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {

            builder.HasKey(x => x.Id); 
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Balance)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasOne(x => x.Agent)
                .WithOne(x => x.Wallet)
                .HasForeignKey<Wallet>(x => x.AgentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.Wallet)
                .HasForeignKey(x => x.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.AgentId)
                .IsUnique();
        }
    }
}
