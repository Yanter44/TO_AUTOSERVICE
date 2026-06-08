using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class ModeratorConfiguration : IEntityTypeConfiguration<ModeratorProfile>
    {
        public void Configure(EntityTypeBuilder<ModeratorProfile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();
        }
    }
}
