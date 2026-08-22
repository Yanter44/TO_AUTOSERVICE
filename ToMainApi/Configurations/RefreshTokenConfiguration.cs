using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.UserId);

            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.HasIndex(x => x.Hash).IsUnique();
        }
    }
}
