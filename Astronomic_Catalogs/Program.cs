using Astronomic_Catalogs.Areas.Services;
using Astronomic_Catalogs.Authorization;
using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Infrastructure;
using Astronomic_Catalogs.Infrastructure.Interfaces;
using Astronomic_Catalogs.Infrastructure.NLogIfrastructure;
using Astronomic_Catalogs.Models.Services;
using Astronomic_Catalogs.Services;
using Astronomic_Catalogs.Services.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.RateLimiting;

namespace Astronomic_Catalogs;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = LoadConfiguration();

        ConfigureLogging(builder);
        AddServices(builder);
        AddDatabaseServices(builder);
        AddIdentityServices(builder, configuration);
        ConfigureRateLimiting(builder);

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        var app = builder.Build();

        ConfigureServices(app, builder);
        ConfigureMiddleware(app, builder);
        ConfigureRoutes(app);

        await InitializeDatabaseAsync(app);
        await app.RunAsync();
    }


    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
#if !DEBUG
        builder.Logging.ClearProviders();
#elif (DEBUG)
        if (builder.Environment.IsDevelopment())   
            TestNLogFileCreating();
#endif
        builder.Host.UseNLog();
    }

    private static void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddTransient<PublicIpService>();
        builder.Services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
        builder.Services.AddSingleton<INLogConfiguration, NLogConfiguration>();
        builder.Services.AddSingleton<NLogConfigProvider>();
        builder.Services.AddScoped<DatabaseInitializer>();
    }

    private static IConfiguration LoadConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static void AddDatabaseServices(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();
            string connectionString = connectionStringProvider.ConnectionString;
            options.UseSqlServer(connectionString);
#if DEBUG
            LogEnvironmentDetails(builder.Environment, connectionString);
#endif
        });
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    }

    private static void AddIdentityServices(WebApplicationBuilder builder, IConfiguration configuration)
    {
        var authSettings = new AuthenticationSettingsProvider(configuration);
        builder.Services.AddSingleton(authSettings);
        builder.Services.AddScoped<UserControllerService>();
        builder.Services.AddScoped<RoleControllerService>();
        builder.Services.AddScoped<JwtService>();
        builder.Services.AddScoped<UserRegister>();
        builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
        // TODO: Generate Identity UI (Razor Pages for Identity) in the project:
        //       dotnet aspnet-codegenerator identity -dc ApplicationDbContext
        builder.Services.AddIdentity<Models.AspNetUser, Models.AspNetRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultUI()
        .AddDefaultTokenProviders(); // Adds tokens for password reset, email confirmation, etc.
        ConfigIdentity(builder, authSettings);
    }

    private static void ConfigIdentity(WebApplicationBuilder builder, AuthenticationSettingsProvider authSettings)
    {
        builder.Services.AddAuthentication(options =>
        {
            // Main scheme – Identity wich contein Coocies scheme.  
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            // Redirects to the login page by default  
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            // If access is denied – redirects to AccessDenied 
            options.DefaultForbidScheme = IdentityConstants.ApplicationScheme;
        })
        #region Cookies and JWT
        .AddCookie(options =>
        {
            options.Cookie.Name = "AstroCatalogsAppCookie";
            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            options.SlidingExpiration = true;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // Set the validation options.
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = authSettings.JwtIssuer,
                ValidAudience = authSettings.JwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.JwtKey))
            };
        })
        #endregion
        #region External accounts
        .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = authSettings.GoogleClientId;
            googleOptions.ClientSecret = authSettings.GoogleClientSecret;
            googleOptions.ClaimActions.MapJsonKey("picture", "picture");
        })
        .AddMicrosoftAccount(microsoftOptions =>
        {
            microsoftOptions.ClientId = authSettings.MicrosoftClientId;
            microsoftOptions.ClientSecret = authSettings.MicrosoftClientSecret;
            microsoftOptions.ClaimActions.MapJsonKey("urn:microsoftaccount:picture", "picture");
        })
        /// TODO:
        ///.AddApple(appleOptions =>
        ///{
        ///    appleOptions.ClientId = authSettings.Apple:ClientId"];
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
        #region Identity Options
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 _-.@";
            options.User.RequireUniqueEmail = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        });
        ConfigureEmailService(builder, authSettings);
        ConfigureAuthorizationPolicies(builder, authSettings);
        #endregion
#if DEBUG
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "AstroCatalogsAppCookie";
            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            options.SlidingExpiration = true;
        });
#endif
    }

    private static void ConfigureEmailService(WebApplicationBuilder builder, AuthenticationSettingsProvider authSettings)
    {
        builder.Services.AddTransient<ICustomEmailSender, EmailSender>(); // Use DummyEmailSender for devolopment
        builder.Services.Configure<AuthMessageSenderOptions>(options =>
        {
            options.Email = authSettings.AuthMessageSenderOptions.Email;
            options.Password = authSettings.AuthMessageSenderOptions.Password;
        });
    }

    private static void ConfigureAuthorizationPolicies(WebApplicationBuilder builder, AuthenticationSettingsProvider authSettings)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("Department", "HQ"));
            options.AddPolicy("UsersAccessClaim", policy => policy.RequireClaim("CanUsersAccess", "true"));
            options.AddPolicy("RoleAccessClaim", policy => policy.RequireClaim("CanRoleAccess", "true"));
            options.AddPolicy("RoleModifyClaim", policy => policy.RequireClaim("CanRoleModify", "true"));
            options.AddPolicy("RoleWatchClaim", policy => policy.RequireClaim("CanRoleWatch", "true"));
            options.AddPolicy("OverAge", policy =>
                policy.Requirements.Add(new MinimumAgeRequirement(authSettings.MinAgeRestriction)));
        });
    }

    /// <summary>
    /// Creating partitions on client IP addresses makes the app vulnerable to Denial of Service Attacks which employ IP Source Address 
    /// Spoofing.For more information, see BCP 38 RFC 2827 Network Ingress Filtering: 
    /// Defeating Denial of Service Attacks which employ IP Source Address Spoofing. - https://www.rfc-editor.org/info/bcp38
    /// </summary>
    private static void ConfigureRateLimiting(WebApplicationBuilder builder)
    {
        #region For registered useers
        builder.Services.AddRateLimiter(options =>
        {
            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                var key = context.HttpContext.User.Identity?.Name ?? "Unknown";
                if (builder.Environment.IsDevelopment())
                    Console.WriteLine($"Rate Limit Triggered! PartitionKey: {key}");

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync("Too Many Requests. Try again later.", cancellationToken);
            };

            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
            #region For registered users
                CreateFixedWindowLimiter(),
                CreateSlidingWindowLimiter(90, TimeSpan.FromMinutes(10), 2),
                CreateSlidingWindowLimiter(600, TimeSpan.FromHours(1), 4),
            #endregion
            #region For unregistered useers
                CreateTokenBucketLimiter(10, 1, TimeSpan.FromSeconds(10), 1),
                CreateTokenBucketLimiter(70, 0, TimeSpan.FromMinutes(10), 1),
                CreateTokenBucketLimiter(400, 0, TimeSpan.FromHours(1), 1)
            #endregion
            );
        });
        #endregion
    }

    private static PartitionedRateLimiter<HttpContext> CreateFixedWindowLimiter()
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                string user = GetUserKey(context);
                return RateLimitPartition.GetFixedWindowLimiter(user, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 15,
                    QueueLimit = 2,
                    Window = TimeSpan.FromSeconds(10)
                });
            }
            return RateLimitPartition.GetNoLimiter("Unauthenticated_user");
        });
    }

    private static PartitionedRateLimiter<HttpContext> CreateSlidingWindowLimiter(
        int permitLimit, TimeSpan window, int segmentsPerWindow)
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                string user = GetUserKey(context);
                return RateLimitPartition.GetSlidingWindowLimiter(user, _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = window,
                    SegmentsPerWindow = segmentsPerWindow
                });
            }
            return RateLimitPartition.GetNoLimiter("Unauthenticated_user");
        });
    }

    private static PartitionedRateLimiter<HttpContext> CreateTokenBucketLimiter(
        int tokenLimit, int queueLimit, TimeSpan replenishmentPeriod, int tokensPerPeriod)
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            if (!(context.User.Identity?.IsAuthenticated ?? true))
            {
                string ip = GetUserKey(context);
                return RateLimitPartition.GetTokenBucketLimiter(ip, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = tokenLimit,
                    QueueLimit = queueLimit,
                    ReplenishmentPeriod = replenishmentPeriod,
                    TokensPerPeriod = tokensPerPeriod
                });
            }
            return RateLimitPartition.GetNoLimiter("Unknown_ip");
        });
    }

    private static string GetUserKey(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            return context.User.Identity.Name!;
        }
        string? publicIp = context.Items["PublicIp"]?.ToString();

        return string.IsNullOrEmpty(publicIp) ? "anonymous_user" : publicIp;
    }

    private static void ConfigureServices(WebApplication app, WebApplicationBuilder builder)
    {

        using (var scope = app.Services.CreateScope())
        {
            var nlogService = scope.ServiceProvider.GetRequiredService<NLogConfigProvider>();
            nlogService.ConfigureLogger();
        }

        if (app.Environment.IsDevelopment())
        {
            TestConfigurationUpoaded();
        }
    }

    private static async Task InitializeDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await dbInitializer.InitializeDatabaseAsync();
    }

    private static void ConfigureMiddleware(WebApplication app, WebApplicationBuilder builder)
    {
        app.Use(async (context, next) =>
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            if (string.IsNullOrWhiteSpace(userAgent))
                userAgent = "Unknown_user";
            var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? userAgent;
            var publicIpService = context.RequestServices.GetRequiredService<PublicIpService>();
            await publicIpService.GetPublicIpAsync(context, partitionKey);
            await next();
        });

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
        app.UseRateLimiter();
        app.UseMiddleware<UserLoggingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<UserAccessMiddleware>();


    }

    private static void ConfigureRoutes(WebApplication app)
    {
        app.MapControllers();


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
