using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class ApplicationDocumentConfiguration: IEntityTypeConfiguration<ApplicationDocument>
    {
        public void Configure(EntityTypeBuilder<ApplicationDocument> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(x => x.Type)
                .IsRequired();

            builder.HasOne(x => x.Application)
                .WithMany(x => x.Documents)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ApplicationId);

            builder.HasIndex(x => x.Type);
        }
    }
}
