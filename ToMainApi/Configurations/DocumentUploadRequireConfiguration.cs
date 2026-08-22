using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToMainApi.Models.Entities;

namespace ToMainApi.Configurations
{
    public class DocumentUploadRequireConfiguration : IEntityTypeConfiguration<DocumentUploadRequire>
    {
        public void Configure(EntityTypeBuilder<DocumentUploadRequire> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            builder.Property(u => u.DisplayName).HasMaxLength(255);
            
        }
    }
}
