using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class AgentBranchConfiguration : IEntityTypeConfiguration<AgentBranch>
    {
        public void Configure(EntityTypeBuilder<AgentBranch> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedOnAdd();

            builder.Property(b => b.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(b => b.Fee)
                .HasPrecision(18, 2);

            builder.HasIndex(b => b.OwnerAgentId);

            builder.HasOne(b => b.Owner)
                .WithMany(a => a.OwnedBranches)
                .HasForeignKey(b => b.OwnerAgentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
