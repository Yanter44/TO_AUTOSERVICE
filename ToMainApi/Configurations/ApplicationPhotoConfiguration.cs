using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class ApplicationPhotoConfiguration : IEntityTypeConfiguration<ApplicationPhoto>
    {
        public void Configure(EntityTypeBuilder<ApplicationPhoto> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(x => x.VehiclePhotoType)
                .IsRequired();

            builder.HasOne(x => x.Application)
                .WithMany(x => x.Photos)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ApplicationId);

            builder.HasIndex(x => x.VehiclePhotoType);
        }
    }
}
