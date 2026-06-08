using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class AgentConfiguration : IEntityTypeConfiguration<AgentProfile>
    {
        public void Configure(EntityTypeBuilder<AgentProfile> builder)
        {
           builder.HasKey(x => x.Id);
           builder.Property(u => u.Id).ValueGeneratedOnAdd();       
        }
    }
}
