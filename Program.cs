using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Infrastructure;
using Astronomic_Catalogs.Infrastructure.NLogIfrastructure;
using Astronomic_Catalogs.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using NLog;
using NLog.Web;
using Astronomic_Catalogs.Models.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Astronomic_Catalogs.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Astronomic_Catalogs;

public class Program
{
    public static async Task Main(string[] args)
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

        // Add services to the container. 
        builder.Services.AddSingleton<INLogConfiguration, NLogConfiguration>();
        builder.Services.AddSingleton<NLogConfigProvider>();
        builder.Services.AddScoped<DatabaseInitializer>();
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

        #region IDENTITY
        builder.Services.AddScoped<UserControllerService>();
        builder.Services.AddScoped<RoleControllerService>();
        builder.Services.AddScoped<JwtService>();
        // TODO: Generate Identity UI (Razor Pages for Identity) in the project:
        //       dotnet aspnet-codegenerator identity -dc ApplicationDbContext
        builder.Services.AddIdentity<Models.AspNetUser, Models.AspNetRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>() 
        .AddDefaultTokenProviders(); // Adds tokens for password reset, email confirmation, etc.

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; - ÎÑÊ²ËÜÊÈ ÍÈÆ×Å:  Google as default
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        });

        #region External accounts
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Google as default
        })
        .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]
             ?? throw new InvalidOperationException("Google ClientId is missing.");
            googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
              ?? throw new InvalidOperationException("Google ClientSecret is missing.");
            googleOptions.ClaimActions.MapJsonKey("picture", "picture");
        })
        .AddMicrosoftAccount(microsoftOptions =>
        {
            microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]
              ?? throw new InvalidOperationException("Microsoft ClientId is missing."); 
            microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]
              ?? throw new InvalidOperationException("Microsoft ClientSecret is missing.");
            microsoftOptions.ClaimActions.MapJsonKey("urn:microsoftaccount:picture", "picture");
        })
        /// TODO:
        ///.AddApple(appleOptions =>
        ///{
        ///    appleOptions.ClientId = builder.Configuration["Authentication:Apple:ClientId"];
        ///    appleOptions.ClientSecret = builder.Configuration["Authentication:Apple:ClientSecret"];
        ///})
        ///.AddOAuth("Phone", options =>
        ///{
        ///    options.ClientId = builder.Configuration["Authentication:Phone:ClientId"];
        ///    options.ClientSecret = builder.Configuration["Authentication:Phone:ClientSecret"];
        ///    options.AuthorizationEndpoint = "https://example.com/oauth/authorize";
        ///    options.TokenEndpoint = "https://example.com/oauth/token";
        ///    options.CallbackPath = "/signin-phone";
        ///})
        ;
        #endregion
        #region Cookies and JWT
        builder.Services.AddAuthentication()
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.SlidingExpiration = true;
        })
        .AddJwtBearer(options =>
        {
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // Set the validation options.
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
            };
        });
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 _-.";
        });
        #endregion
        #region Email
        builder.Services.AddTransient<ICustomEmailSender, EmailSender>(); // Use DummyEmailSender for devolopment
        builder.Services.AddOptions();
        builder.Services.Configure<AuthMessageSenderOptions>(
            builder.Configuration.GetSection("AuthMessageSenderOptions")
            );

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.RequireUniqueEmail = true;
        });
        #endregion
        #endregion

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();




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

        using (var scope = app.Services.CreateScope())
        {
            var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            await dbInitializer.InitializeDatabaseAsync();
        }

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

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=HomeAdmin}/{action=Index}/{id?}"
        );
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}"
        );

        app.MapRazorPages();

#if DEBUG
        app.Use(async (context, next) =>
        {
            Console.WriteLine($"Requested Path: {context.Request.Path}");
            await next.Invoke();
        });
#endif

        await app.RunAsync();
    }

    private static void LogEnvironmentDetails(IWebHostEnvironment environment, string connectionString)
    {
        Console.WriteLine($"\n\n  USING ENVIRONMENT NAME: {environment.EnvironmentName}");
        Console.WriteLine($"\n\n  USING CONNECTION STRING: {connectionString} \n\n");
    }

    // TODO: Make test
    private static void TestNLogFileCreating()
    {
        var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "nlog-internal.log");
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
        Console.WriteLine($"Connection rules count: {config.LoggingRules.Count}\n");
        foreach (var target in config.AllTargets)
        {
            Console.WriteLine($"Target name: {target.Name}, type: {target.GetType()}");
        }
        Console.WriteLine("\n\n");
    }
}
