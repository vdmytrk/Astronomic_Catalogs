using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Infrastructure;
using Astronomic_Catalogs.Infrastructure.NLogIfrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

namespace Astronomic_Catalogs;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        string connectionString = "";

        builder.Services.AddSingleton<ConnectionStringProvider>();

#if !DEBUG
        builder.Logging.ClearProviders();
#elif (DEBUG)
        if ("Development" == builder.Environment.EnvironmentName)
            TestNLogFileCreating();
#endif
        builder.Host.UseNLog();
        // Add services to the container. TEST COMMIT 2025.02.03 11:52 При підключенні до Astronomic Project\AstronomicSolution\AstronomicSolution.sln
        builder.Services.AddSingleton<INLogConfiguration, NLogConfiguration>();
        builder.Services.AddSingleton<NLogConfigProvider>();
        builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionStringProvider = serviceProvider.GetRequiredService<ConnectionStringProvider>();
            connectionString = connectionStringProvider.ConnectionString;
            options.UseSqlServer(connectionString);
#if DEBUG
            LogEnvironmentDetails(builder.Environment, connectionStringProvider.ConnectionString);
#endif
        });

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();




        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var nlogService = scope.ServiceProvider.GetRequiredService<NLogConfigProvider>();
            nlogService.ConfigureLogger();
        }
#if (DEBUG)
        if ("Development" == builder.Environment.EnvironmentName)
            TestConfigurationUpoaded();
#endif

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

        app.Run();
    }

    private static void LogEnvironmentDetails(IWebHostEnvironment environment, string connectionString)
    {
        Console.WriteLine($"\n\n  USING ENVIRONMENT NAME: {environment.EnvironmentName}");
        Console.WriteLine($"\n\n  USING CONNECTION STRING: {connectionString} \n\n");
    }

    // TODO: Make test
    private static void TestNLogFileCreating()
    {
        var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "nlog-internal_xml.log");
        try
        {
            File.WriteAllText(logFilePath, "\n\n Log file created.\n\n");
            Console.WriteLine($"\n\nLog file created successfully at: {logFilePath}\n\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n\nFailed to create log file. Error: {ex.Message}");
        }
        Console.WriteLine(new string('-', 50));
    }

    private static void TestConfigurationUpoaded()
    {
        var config = LogManager.Configuration;
        Console.WriteLine($"\n\nTargets loaded: {config.AllTargets.Count}"); 
        Console.WriteLine($"Logging rules count: {config.LoggingRules.Count}\n"); 
        foreach (var target in config.AllTargets)
        {
            Console.WriteLine($"Target name: {target.Name}, type: {target.GetType()}");
        }
        Console.WriteLine("\n\n");
    }
}
