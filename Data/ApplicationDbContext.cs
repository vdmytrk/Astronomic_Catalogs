using Astronomic_Catalogs.Infrastructure;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Models.Configuration;
using Astronomic_Catalogs.Models.Configuration.Connection;
using Astronomic_Catalogs.Models.Connection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Astronomic_Catalogs.Areas.Admin.Models;

namespace Astronomic_Catalogs.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    public DbSet<ActualDate> ActualDates { get; set; } = null!;
    public DbSet<SourceType> SourceTypes { get; set; } = null!;    
    public DbSet<NameObject> NameObjects { get; set; } = null!;
    public DbSet<Constellation> Constellations { get; set; } = null!;
    public DbSet<CollinderCatalog> CollinderCatalog { get; set; } = null!;
    public DbSet<NGCICOpendatasoft> NGCIC_Catalog { get; set; } = null!;
    public DbSet<NGCICOpendatasoftExtension> NGCICOpendatasoft_E { get; set; } = null!;
    public DbSet<LogProcFunc> LogProcFuncs { get; set; } = null!;
    public DbSet<TestConnectionForNLog> TestConnectionForNLogs { get; set; } = null!;
    public DbSet<NLogApplicationCode> NLogApplicationCodes { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ActualDate>().ToTable("DateTable");
        modelBuilder.Entity<NameObject>(entity =>
            entity.HasNoKey()
        );
        modelBuilder.Entity<SourceType>(entity =>
            entity.HasNoKey()
        );

        modelBuilder.ApplyConfiguration(new CollinderCatalogConfiguration());
        modelBuilder.ApplyConfiguration(new ConstellationConfiguration());
        modelBuilder.ApplyConfiguration(new NameObjectConfiguration());
        modelBuilder.ApplyConfiguration(new NGCICOpendatasoftConfiguration());
        modelBuilder.ApplyConfiguration(new NGCICOpendatasoftExtensionConfiguration());
        modelBuilder.ApplyConfiguration(new NASAExoplanetCatalogConfiguration());
        modelBuilder.ApplyConfiguration(new SourceTypeConfiguration());
        modelBuilder.ApplyConfiguration(new LogProcFuncConfiguration());
        modelBuilder.ApplyConfiguration(new NLogApplicationCodeConfiguration());
        modelBuilder.ApplyConfiguration(new TestConnectionForNLogConfiguration());

    }

public DbSet<Astronomic_Catalogs.Areas.Admin.Models.AspNetRole> AspNetRole { get; set; } = default!;

public DbSet<Astronomic_Catalogs.Areas.Admin.Models.AspNetRoleClaim> AspNetRoleClaim { get; set; } = default!;

public DbSet<Astronomic_Catalogs.Areas.Admin.Models.AspNetUser> AspNetUser { get; set; } = default!;

public DbSet<Astronomic_Catalogs.Areas.Admin.Models.AspNetUserClaim> AspNetUserClaim { get; set; } = default!;

public DbSet<Astronomic_Catalogs.Areas.Admin.Models.AspNetUserLogin> AspNetUserLogin { get; set; } = default!;

public DbSet<Astronomic_Catalogs.Areas.Admin.Models.AspNetUserRole> AspNetUserRole { get; set; } = default!;

public DbSet<Astronomic_Catalogs.Areas.Admin.Models.AspNetUserToken> AspNetUserToken { get; set; } = default!;
}

public class DbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[]? args = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        var connectionStringProvider = new ConnectionStringProvider(configuration);
        var connectionString = connectionStringProvider.ConnectionString;

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
