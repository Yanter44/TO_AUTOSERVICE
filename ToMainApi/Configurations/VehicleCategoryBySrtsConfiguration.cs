using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class VehicleCategoryBySrtsConfiguration : IEntityTypeConfiguration<VehicleCategoryBySrts>
    {
        public void Configure(EntityTypeBuilder<VehicleCategoryBySrts> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .IsRequired()             
                .HasMaxLength(100);        

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
