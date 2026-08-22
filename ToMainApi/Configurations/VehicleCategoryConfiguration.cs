using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class VehicleCategoryConfiguration : IEntityTypeConfiguration<VehicleCategory>
    {
        public void Configure(EntityTypeBuilder<VehicleCategory> builder)
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
