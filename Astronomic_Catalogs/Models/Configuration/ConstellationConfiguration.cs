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

        // You used `UseCollation` because changing the collation setting for the entire database didn’t work. For more details, see the SQL scripts!
        builder.Property(e => e.UkraineName)
            .HasColumnName("Ukraine_name")
            .HasColumnType("nvarchar(30)")
            .UseCollation("Cyrillic_General_CI_AS")
            .IsRequired();

        builder.Property(e => e.NumberStars).HasColumnName("Number_stars");
    }
}
