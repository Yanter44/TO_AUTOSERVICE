using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.VIN)
                .IsRequired()
                .HasMaxLength(17);

            builder.Property(x => x.GosNumber)
                .HasMaxLength(15);

            builder.Property(x => x.Brand)
                .HasMaxLength(100);

            builder.Property(x => x.Model)
                .HasMaxLength(100);

            builder.Property(x => x.YearOfRelease)
                .IsRequired();

            builder.Property(x => x.VehicleCategoryId)
                .IsRequired();

            builder.Property(x => x.FIO)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(x => x.Pto)
                .WithMany()
                .HasForeignKey(x => x.PtoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.PtoId)
                .IsRequired();

            builder.HasOne(x => x.VehicleCategory)
                .WithMany()
                .HasForeignKey(x => x.VehicleCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Documents)
                .WithOne(x => x.Application)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Photos)
                .WithOne(x => x.Application)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.VIN);

            builder.HasIndex(x => x.PtoId);

            builder.HasIndex(x => x.VehicleCategoryId);

            builder.HasIndex(x => x.Email);

            builder.HasIndex(x => x.GosNumber);
        }
    }
}
