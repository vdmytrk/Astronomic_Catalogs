using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Models.Configuration;

public class CollinderCatalogConfiguration : IEntityTypeConfiguration<CollinderCatalog>
{
    public void Configure(EntityTypeBuilder<CollinderCatalog> builder)
    {
        builder.ToTable("CollinderCatalog");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.NamberName)
            .HasColumnName("Namber_name")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.NameOtherCat).HasMaxLength(40);
        builder.Property(e => e.Constellation).HasMaxLength(5);


        builder.Property(e => e.RightAscension).HasColumnName("Right_ascension").HasMaxLength(15);
        builder.Property(e => e.RightAscensionH).HasColumnName("Right_ascension_H");
        builder.Property(e => e.RightAscensionM).HasColumnName("Right_ascension_M");
        builder.Property(e => e.RightAscensionS).HasColumnName("Right_ascension_S");


        builder.Property(e => e.Declination).HasMaxLength(15).HasColumnName("Declination");
        builder.Property(e => e.NS).HasMaxLength(1);
        builder.Property(e => e.DeclinationD).HasColumnName("Declination_D");
        builder.Property(e => e.DeclinationM).HasColumnName("Declination_M");
        builder.Property(e => e.DeclinationS).HasColumnName("Declination_S");


        builder.Property(e => e.AppMag).HasColumnName("App_Mag");
        builder.Property(e => e.AppMagFlag).HasMaxLength(3).HasColumnName("App_Mag_Flag");


        builder.Property(e => e.CountStars).HasMaxLength(10);
        builder.Property(e => e.CountStarsToFinding).HasColumnName("CountStars_ToFinding");
        builder.Property(e => e.AngDiameter).HasMaxLength(10).HasColumnName("Ang_Diameter");
        builder.Property(e => e.AngDiameterNew).HasColumnName("Ang_Diameter_Max");
        builder.Property(e => e.Class).HasMaxLength(10);


        builder.Property(e => e.RowOnPage).HasDefaultValue(1);
    }
}
