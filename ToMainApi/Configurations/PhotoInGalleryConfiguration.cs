using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class PhotoInGalleryConfiguration : IEntityTypeConfiguration<PhotoInGallery>
    {
        public void Configure(EntityTypeBuilder<PhotoInGallery> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.Tag)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Group)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Url)
                .HasMaxLength(900)
                .IsRequired();

            builder.Property(p => p.PublicId)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.HasIndex(p => p.PublicId).IsUnique();

            builder.HasIndex(p => p.Group);
            builder.HasIndex(p => p.Tag);
            builder.HasIndex(p => p.CreatedAt).IsDescending();
        }
    }
}
