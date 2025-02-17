using Astronomic_Catalogs.Infrastructure;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Models.Configuration;
using Astronomic_Catalogs.Models.Configuration.Connection;
using Astronomic_Catalogs.Models.Connection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
//using Astronomic_Catalogs.Areas.Admin.Models;

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

    //public DbSet<AspNetUser> Users { get; set; } // For create controller

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

        #region Identity
        modelBuilder.Entity<Models.AspNetRole>()
            .HasMany(r => r.RoleClaims)
            .WithOne(rc => rc.Role)
            .HasForeignKey(rc => rc.RoleId);

        modelBuilder.Entity<Models.AspNetRole>()
            .HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId);



        modelBuilder.Entity<Models.AspNetUser>()
            .HasMany(r => r.UserClaims)
            .WithOne(rc => rc.User)
            .HasForeignKey(rc => rc.UserId);

        modelBuilder.Entity<Models.AspNetUser>()
            .HasMany(r => r.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<Models.AspNetUser>()
            .HasMany(r => r.UserLogins)
            .WithOne(rc => rc.User)
            .HasForeignKey(rc => rc.UserId);

        modelBuilder.Entity<Models.AspNetUser>()
            .HasMany(r => r.UserTokens)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);

        // Вимкнення стандартних таблиць Identity
        //modelBuilder.Entity<IdentityUser>().ToTable(null); // Це допомагає EF не плутати з вашою таблицею
        #endregion


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
