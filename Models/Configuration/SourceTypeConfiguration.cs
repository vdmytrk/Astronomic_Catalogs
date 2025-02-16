using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Models.Configuration;

public class SourceTypeConfiguration : IEntityTypeConfiguration<SourceType>
{
    public void Configure(EntityTypeBuilder<SourceType> builder)
    {
        builder.ToTable("SourceType");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Count)
            .HasColumnName("Count"); // The name matches, but "Count" is a reserved word, so explicit specification is appropriate.
                                     
        builder.Property(e => e.Code)
            .HasMaxLength(5);

        builder.Property(e => e.Meaning)
            .HasMaxLength(200);
    }
}
