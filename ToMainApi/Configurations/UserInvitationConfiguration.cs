using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class UserInvitationConfiguration : IEntityTypeConfiguration<UserInvitation>
    {
        public void Configure(EntityTypeBuilder<UserInvitation> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.RoleType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(64);

            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(64); 

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.UsedAt)
                .IsRequired(false);

            builder.Property(x => x.CreatedByUserId)
                .IsRequired();

            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Token)
                .IsUnique()
                .HasDatabaseName("IX_UserInvitations_Token");

            builder.HasIndex(x => x.Email)
                .HasDatabaseName("IX_UserInvitations_Email");

            builder.HasIndex(x => x.CreatedByUserId)
                .HasDatabaseName("IX_UserInvitations_CreatedByUserId");
        }
    }
}
