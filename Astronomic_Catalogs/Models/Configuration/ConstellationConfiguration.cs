using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Models.Configuration;

public class ConstellationConfiguration : IEntityTypeConfiguration<Constellation>
{
    public void Configure(EntityTypeBuilder<Constellation> builder)
    {
        builder.ToTable("Constellation");

        builder.HasKey(e => e.ShortName);

        builder.Property(e => e.ShortName)
            .HasColumnName("Short_name")
            .HasMaxLength(3);

        builder.Property(e => e.LatineNameNominativeCase)
            .HasColumnName("Latine_name_Nominative_case")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.LatineNameGenitiveCase)
            .HasColumnName("Latine_name_Genitive_case")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.UkraineName)
            .HasColumnName("Ukraine_name")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.NumberStars).HasColumnName("Number_stars");
    }
}
