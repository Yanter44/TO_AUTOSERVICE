using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Infrastructure.Persistence.Configurations;

public class PtoOperatorCryptoKeyConfiguration: IEntityTypeConfiguration<PtoOperatorCryptoKey>
{
    public void Configure(EntityTypeBuilder<PtoOperatorCryptoKey> b)
    {
        b.HasKey(x => x.PtoId);

        b.Property(x => x.EncryptedData)
            .HasColumnType("bytea")
            .IsRequired();

        b.Property(x => x.CreatedAt)
         .IsRequired();

        b.HasOne(x => x.Pto)
         .WithOne(p => p.OperatorCryptoKey)
         .HasForeignKey<PtoOperatorCryptoKey>(x => x.PtoId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}