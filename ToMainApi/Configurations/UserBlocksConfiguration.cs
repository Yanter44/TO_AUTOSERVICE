using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class UserBlockConfiguration : IEntityTypeConfiguration<UserBlocks>
    {
        public void Configure(EntityTypeBuilder<UserBlocks> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(b => b.User)
                .WithMany(u => u.Blocks)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.BlockedBy)
                .WithMany()
                .HasForeignKey(b => b.BlockedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.UnblockedBy)
                .WithMany()
                .HasForeignKey(b => b.UnblockedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(b => b.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(b => b.BlockedAt)
                .IsRequired();

            builder.Property(b => b.BlockedUntil);

            builder.Property(b => b.UnblockedAt);

            builder.Property(b => b.UnblockReason)
                .HasMaxLength(500);

            builder.HasIndex(b => b.UserId);
            builder.HasIndex(b => b.BlockedAt);
            builder.HasIndex(b => new { b.UserId, b.UnblockedAt });
        }

    }
}
