using Astronomic_Catalogs.Infrastructure;
using NLog;
using NLog.Web;

namespace Astronomic_Catalogs;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = LogManager.Setup()
            .LoadConfigurationFromFile("NLog.config.json", optional: false)
            .GetCurrentClassLogger();

        if (LogManager.Configuration == null)
        {
            Console.WriteLine("NLog configuration not loaded.");
        }
        else
        {
            Console.WriteLine("NLog configuration loaded successfully.");
            Console.WriteLine($"IsErrorEnabled: {logger.IsErrorEnabled}");
        }

        try
        {
            var builder = WebApplication.CreateBuilder(args);

#if (!DEBUG)
            builder.Logging.ClearProviders();
#endif
            builder.Host.UseNLog();

            builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

        catch (Exception ex)
        {
            logger.Error(ex, "Stopped program because of an exception");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }

    }
}
