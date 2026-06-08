using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<AdminProfile>
    {
        public void Configure(EntityTypeBuilder<AdminProfile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();
        }
    }
}
