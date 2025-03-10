using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Models.Configuration;

public class NLogApplicationCodeConfiguration : IEntityTypeConfiguration<NLogApplicationCode>
{
    public void Configure(EntityTypeBuilder<NLogApplicationCode> builder)
    {
        builder.ToTable("NLogApplicationCode");

        builder.HasKey(e => e.Id).HasName("PK_NLog");
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.CreatedOn)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(e => e.Logged)
            .IsRequired();

        builder.Property(e => e.Level)
            .HasColumnType("varchar(10)");

        builder.Property(e => e.Ip)
            .HasColumnType("varchar(50)");

        builder.Property(e => e.MachineName)
            .HasColumnType("nvarchar(255)");

        builder.Property(e => e.SessionId)
            .HasColumnType("nvarchar(255)");

        builder.Property(e => e.Logger)
            .HasColumnType("varchar(300)");

        builder.Property(e => e.Controller)
            .HasColumnType("varchar(100)");

        builder.Property(e => e.Action)
            .HasColumnType("varchar(50)");

        builder.Property(e => e.Method)
            .HasColumnType("varchar(300)");

        builder.Property(e => e.Exception)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.ActivityId)
            .HasColumnType("nvarchar(50)");

    }
}
