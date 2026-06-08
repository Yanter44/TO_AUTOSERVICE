using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.FIO)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(u => u.Password)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}
