using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Astronomic_Catalogs.Models.Loging;
using Microsoft.EntityFrameworkCore.Design;
using Astronomic_Catalogs.Infrastructure;

namespace Astronomic_Catalogs.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<NLogLogings> Products { get; set; }
}





//public class DbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
//{
//    public ApplicationDbContext CreateDbContext(string[]? args = null)
//    {
//        // Manually initialize the ConnectionStringProvider
//        var configuration = new ConfigurationBuilder()
//            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
//            .AddEnvironmentVariables()
//            .Build();
//        var connectionStringProvider = new ConnectionStringProvider(configuration);
//        var connectionString = connectionStringProvider.ConnectionString;

//        // Original
//        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//        optionsBuilder.UseSqlServer(connectionString);

//        // Original
//        //optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-4MV9DF1\SQLEXPRESS;Initial Catalog=AstroCatalogs;Integrated Security=True;Encrypt=False;MultipleActiveResultSets=True;Max Pool Size=200;Password=123vovkUlaka456N2;");

//        return new ApplicationDbContext(optionsBuilder.Options);
//    }
//}
