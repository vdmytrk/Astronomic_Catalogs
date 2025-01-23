using Astronomic_Catalogs.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Infrastructure;

public static class ConfigureServices
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Реєстрація ConnectionStringProvider
        services.AddSingleton<ConnectionStringProvider>();

        // Реєстрація ApplicationDbContext
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionStringProvider = serviceProvider.GetRequiredService<ConnectionStringProvider>();
            var connectionString = connectionStringProvider.ConnectionString;
            options.UseSqlServer(connectionString);

#if DEBUG
            Console.WriteLine($"\n\n  USING ENVIRONMENT NAME: {environment.EnvironmentName}");
            Console.WriteLine($"\n\n  USING CONNECTION STRING: {connectionString} \n\n");
#endif
        });

        // Інші сервіси
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddControllersWithViews();
    }
}

