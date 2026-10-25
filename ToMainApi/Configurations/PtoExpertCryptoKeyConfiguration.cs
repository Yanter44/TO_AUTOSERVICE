using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class PtoExpertCryptoKeyConfiguration : IEntityTypeConfiguration<PtoExpertCryptoKey>
    {
        public void Configure(EntityTypeBuilder<PtoExpertCryptoKey> b)
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.EncryptedData)
             .HasColumnType("bytea")
             .IsRequired();

            b.HasIndex(x => x.ExpertId)
             .IsUnique()
             .HasDatabaseName("UX_PtoExpertCryptoKeys_ExpertId");
        }
    }
}
