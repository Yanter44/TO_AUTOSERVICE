using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class PtoConfiguration : IEntityTypeConfiguration<Pto>
    {
        public void Configure(EntityTypeBuilder<Pto> builder)
        {
            builder.HasKey(x => x.Id); 
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.RsaNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Address)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.Latitude)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Longitude)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Login)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Password)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ApiKey)
                .HasMaxLength(200);

            builder.HasMany(x => x.PricePolicies)
                .WithOne(x => x.Pto)
                .HasForeignKey(x => x.PtoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
