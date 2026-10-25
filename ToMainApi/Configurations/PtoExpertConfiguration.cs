using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class PtoExpertConfiguration : IEntityTypeConfiguration<PtoExpert>
    {
        public void Configure(EntityTypeBuilder<PtoExpert> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.MiddleName).HasMaxLength(100);
            builder.Property(x => x.SequenceNumber).IsRequired();

            builder.HasOne(x => x.Pto)
             .WithMany(p => p.Experts)
             .HasForeignKey(x => x.PtoId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CryptoKey)
             .WithOne(k => k.Expert)
             .HasForeignKey<PtoExpertCryptoKey>(k => k.ExpertId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.PtoId);
        }
    }
}
