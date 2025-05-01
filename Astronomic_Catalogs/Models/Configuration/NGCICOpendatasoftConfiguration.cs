using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Models.Configuration;

public class NGCICOpendatasoftConfiguration : IEntityTypeConfiguration<NGCICOpendatasoft>
{
    public void Configure(EntityTypeBuilder<NGCICOpendatasoft> builder)
    {
        builder.ToTable("NGCICOpendatasoft");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.NGC_IC).HasMaxLength(13);
        builder.Property(e => e.Name).HasColumnName("Name"); // The name matches, but "Name" is a reserved word, so explicit specification is appropriate.
        builder.Property(e => e.SubObject).HasMaxLength(15);
        builder.Property(e => e.Messier).HasMaxLength(15);
        builder.Property(e => e.Name_UK).HasMaxLength(50);
        builder.Property(e => e.Comment).HasMaxLength(50);
        builder.Property(e => e.OtherNames).HasColumnName("Other_names").HasMaxLength(400);
        builder.Property(e => e.NGC).HasMaxLength(14);
        builder.Property(e => e.IC).HasMaxLength(23);
        builder.Property(e => e.LimitAngDiameter).HasColumnName("Limit_Ang_Diameter").HasMaxLength(1);
        builder.Property(e => e.AngDiameter).HasColumnName("Ang_Diameter");
        builder.Property(e => e.ObjectTypeAbrev).HasMaxLength(21);
        builder.Property(e => e.ObjectType).HasMaxLength(26);
        builder.Property(e => e.ObjectTypeFull).HasColumnName("Object_type").HasMaxLength(60);
        builder.Property(e => e.SourceType).HasColumnName("Source_Type").HasMaxLength(5);



        builder.Property(e => e.RA).HasMaxLength(30);

        builder.Property(e => e.RightAscension)
            .HasColumnName("Right_ascension")
            .HasMaxLength(15)
            .HasComputedColumnSql(@"
                ISNULL(CAST([Right_ascension_H] AS varchar(10)), '') + 'h ' + 
                ISNULL(CAST([Right_ascension_M] AS varchar(10)), '') + 'm ' + 
                ISNULL(CAST([Right_ascension_S] AS varchar(10)), '') + 's'", stored: true);

        builder.Property(e => e.RightAscensionH).HasColumnName("Right_ascension_H").HasDefaultValue(0);
        builder.Property(e => e.RightAscensionM).HasColumnName("Right_ascension_M").HasDefaultValue(0);
        builder.Property(e => e.RightAscensionS).HasColumnName("Right_ascension_S").HasDefaultValue(0.0);



        builder.Property(e => e.DEC)
            .HasColumnName("DEC") // The name matches, but "DEC" is a reserved word, so explicit specification is appropriate.
            .HasMaxLength(31);

        builder.Property(e => e.Declination)
            .HasColumnName("Declination")
            .HasMaxLength(15)
            .HasComputedColumnSql(@"
                ISNULL(CAST([Declination_D] AS varchar(10)), '') + '° ' + 
                ISNULL(CAST([Declination_M] AS varchar(10)), '') + ''' ' + 
                ISNULL(CAST([Declination_S] AS varchar(10)), '') + '""'", stored: true);
        
        builder.Property(e => e.NS).HasMaxLength(1);
        builder.Property(e => e.DeclinationD).HasColumnName("Declination_D").HasDefaultValue(0);
        builder.Property(e => e.DeclinationM).HasColumnName("Declination_M").HasDefaultValue(0);
        builder.Property(e => e.DeclinationS).HasColumnName("Declination_S").HasDefaultValue(0.0);


        builder.Property(e => e.Constellation).HasMaxLength(21);
        builder.Property(e => e.MajorAxis).HasDefaultValue(0.0);
        builder.Property(e => e.MinorAxis).HasDefaultValue(0.0);
        builder.Property(e => e.PositionAngle).HasDefaultValue(0);


        builder.Property(e => e.AppMag).HasColumnName("App_Mag").HasDefaultValue(0.0);
        builder.Property(e => e.AppMagFlag).HasColumnName("App_Mag_Flag").HasMaxLength(1);
        builder.Property(e => e.BMag).HasColumnName("b_mag").HasDefaultValue(0.0);
        builder.Property(e => e.VMag).HasColumnName("v_mag").HasDefaultValue(0.0);
        builder.Property(e => e.JMag).HasColumnName("j_mag").HasDefaultValue(0.0);
        builder.Property(e => e.HMag).HasColumnName("h_mag").HasDefaultValue(0.0);
        builder.Property(e => e.KMag).HasColumnName("k_mag").HasDefaultValue(0.0);


        builder.Property(e => e.SurfaceBrightness).HasColumnName("Surface_Brigthness").HasDefaultValue(0.0);
        builder.Property(e => e.HubbleOnlyGalaxies).HasColumnName("Hubble_OnlyGalaxies").HasMaxLength(14);
        builder.Property(e => e.CstarUMag).HasColumnName("Cstar_UMag").HasDefaultValue(0.0);
        builder.Property(e => e.CstarBMag).HasColumnName("Cstar_BMag").HasDefaultValue(0.0);
        builder.Property(e => e.CstarVMag).HasColumnName("Cstar_VMag").HasDefaultValue(0.0);
        builder.Property(e => e.CstarNames).HasColumnName("Cstar_Names").HasMaxLength(21);
        builder.Property(e => e.CommonNames).HasMaxLength(110);
        builder.Property(e => e.NedNotes).HasColumnType("varchar(max)");
        builder.Property(e => e.OpenngcNotes).HasColumnType("varchar(max)");
        builder.Property(e => e.Image).HasColumnName("Image"); // The name matches, but "Image" is a reserved word, so explicit specification is appropriate.

        builder.Property(e => e.PageNumber).IsRequired(false);
        builder.Property(e => e.PageCount).IsRequired(false);

        builder.HasIndex(e => new { e.NGC_IC, e.Name })
            .IsUnique()
            .HasDatabaseName("UNIQUE_NGC_IC_Name");
    }
}
