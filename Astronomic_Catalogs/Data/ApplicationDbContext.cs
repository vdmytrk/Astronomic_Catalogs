using Astronomic_Catalogs.Infrastructure;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Models.Configuration;
using Astronomic_Catalogs.Models.Configuration.Connection;
using Astronomic_Catalogs.Models.Configuration.Identity;
using Astronomic_Catalogs.Models.Configuration.Services;
using Astronomic_Catalogs.Models.Connection;
using Astronomic_Catalogs.Models.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Astronomic_Catalogs.Data;

public class ApplicationDbContext : IdentityDbContext
    <
    Models.AspNetUser,
    Models.AspNetRole,
    string, // Type of key           
    Models.AspNetUserClaim,
    Models.AspNetUserRole,
    Models.AspNetUserLogin,
    Models.AspNetRoleClaim,
    Models.AspNetUserToken>
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

    
    public DbSet<AspNetRole> Roles { get; set; } = null!;
    public DbSet<AspNetRoleClaim> RoleClaims { get; set; } = null!;
    public DbSet<AspNetUser> Users { get; set; } = null!;
    public DbSet<AspNetUserClaim> UserClaims { get; set; } = null!;
    public DbSet<AspNetUserLogin> UserLogins { get; set; } = null!;
    public DbSet<AspNetUserToken> UserTokens { get; set; } = null!;

    public DbSet<UserLog> UserLogs { get; set; }
    public DbSet<RequestLog> RequestLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ActualDate>().ToTable("DateTable");
        modelBuilder.Entity<NameObject>().HasNoKey();
        modelBuilder.Entity<SourceType>().HasNoKey();

        modelBuilder.ApplyConfiguration(new RequestLogConfiguration());
        modelBuilder.ApplyConfiguration(new UsersLogConfiguration());

        #region Identity
        modelBuilder.Entity<AspNetUserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
        modelBuilder.ApplyConfiguration(new AspNetRoleConfiguration());
        modelBuilder.ApplyConfiguration(new AspNetUserConfiguration());
        #endregion

        #region Astronomic catalogs
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
        #endregion
    }
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
