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
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Path)
                .HasMaxLength(500)
                .IsRequired();

            builder.HasIndex(x => x.Path);

            builder.HasIndex(x => x.UserId).IsUnique();

            builder.HasOne(x => x.User)
                .WithOne(u => u.AgentProfile)
                .HasForeignKey<AgentProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(x => x.ParentAgent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentAgentId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.Branch)
                 .WithMany()                                 
                 .HasForeignKey(x => x.BranchId)
                 .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.OwnedBranches)
                 .WithOne(b => b.Owner)
                 .HasForeignKey(b => b.OwnerAgentId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.BranchId);
            builder.HasIndex(x => x.ParentAgentId);
        }
    }
}
