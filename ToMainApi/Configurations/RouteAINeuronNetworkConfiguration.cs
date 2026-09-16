using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class RouteAINeuronNetworkConfiguration : IEntityTypeConfiguration<RouteAINeuronNetwork>
    {
        public void Configure(EntityTypeBuilder<RouteAINeuronNetwork> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            
            builder.Property(x => x.Name).HasMaxLength(500);
            builder.Property(x => x.Link).HasMaxLength(150);
        }
    }
}