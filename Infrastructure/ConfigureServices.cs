using Astronomic_Catalogs.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Infrastructure;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        AddConnectionStringProvider(services, environment);

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

        AddNLogProvider(services);

        services.AddControllersWithViews();
    }

    private static void AddConnectionStringProvider(IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddSingleton<ConnectionStringProvider>();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionStringProvider = serviceProvider.GetRequiredService<ConnectionStringProvider>();
            var connectionString = connectionStringProvider.ConnectionString;
            options.UseSqlServer(connectionString);
#if DEBUG
            LogEnvironmentDetails(environment, connectionStringProvider.ConnectionString);
#endif
        });
    }

    private static void AddNLogProvider(IServiceCollection services)
    {
        services.AddSingleton<NLogService>();
        services.AddSingleton(provider =>
        {
            var connectionStringProvider = provider.GetRequiredService<ConnectionStringProvider>();
            var nlogService = provider.GetRequiredService<NLogService>();
            nlogService.UpdateNLogDatabaseConnectionString(connectionStringProvider.ConnectionString);
            return nlogService;
        });
    }

    private static void LogEnvironmentDetails(IWebHostEnvironment environment, string connectionString)
    {
        Console.WriteLine($"\n\n  USING ENVIRONMENT NAME: {environment.EnvironmentName}");
        Console.WriteLine($"\n\n  USING CONNECTION STRING: {connectionString} \n\n");
    }
}


