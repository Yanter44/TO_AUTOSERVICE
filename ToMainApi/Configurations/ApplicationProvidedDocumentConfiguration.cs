using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class ApplicationProvidedDocumentConfiguration : IEntityTypeConfiguration<ApplicationProvidedDocument>
    {
        public void Configure(EntityTypeBuilder<ApplicationProvidedDocument> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.Property(u => u.DisplayName).HasMaxLength(255);
            
        }
    }
}
