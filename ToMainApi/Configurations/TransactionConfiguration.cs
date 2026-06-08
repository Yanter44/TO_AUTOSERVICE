using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.HasOne(x => x.Wallet)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.WalletId);

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
