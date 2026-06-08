using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class PtoPricePolicyConfiguration : IEntityTypeConfiguration<PtoPricePolicy>
    {
        public void Configure(EntityTypeBuilder<PtoPricePolicy> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasOne(x => x.Pto)
                .WithMany(x => x.PricePolicies)
                .HasForeignKey(x => x.PtoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.VehicleCategory)
                .WithMany()
                .HasForeignKey(x => x.VehicleCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.PtoId, x.VehicleCategoryId })
                .IsUnique();
        }
    }
}
