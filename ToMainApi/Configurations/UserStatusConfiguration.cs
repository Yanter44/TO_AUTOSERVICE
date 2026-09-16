using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class UserStatusConfiguration : IEntityTypeConfiguration<UserStatus>
    {
        public void Configure(EntityTypeBuilder<UserStatus> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(s => s.User)
                .WithOne(u => u.Status)
                .HasForeignKey<UserStatus>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => s.UserId)
                .IsUnique();

            builder.Property(s => s.IsBlocked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(s => s.BlockedUntil);

            builder.HasIndex(s => s.IsBlocked);
        }
    }
}
