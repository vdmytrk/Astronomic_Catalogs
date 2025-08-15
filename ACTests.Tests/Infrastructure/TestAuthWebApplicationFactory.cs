using ACTests.Tests.TestUtilities;
using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services;
using Astronomic_Catalogs.Services.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ACTests.Tests.WebApplicationFactory;

public class TestAuthWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    public TestAuthHandlerOptions CurrentUserOptions { get; set; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IUserStore<AspNetUser>>();
                services.RemoveAll<IRoleStore<AspNetRole>>();
                services.RemoveAll<UserManager<AspNetUser>>();
                services.RemoveAll<RoleManager<AspNetRole>>();

                services.RemoveAll<IAuthenticationSchemeProvider>();
                services.RemoveAll<IAuthenticationHandlerProvider>();
                services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();

                services.Remove(
                    services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>))!
                );
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("AuthTestDb");
                });

                services.AddIdentity<AspNetUser, AspNetRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Identity/Account/Login";
                    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                });

                services.RemoveAll<IExcelImport>();
                services.RemoveAll(typeof(IPublicIpService));
                services.AddScoped(_ =>
                {
                    var mockIpService = A.Fake<IPublicIpService>();
                    A.CallTo(() => mockIpService.GetPublicIpAsync(A<HttpContext>._, A<string>._))
                        .Invokes((HttpContext ctx, string ip) =>
                        {
                            ctx.Items["PublicIp"] = "127.0.0.1";
                        })
                        .Returns(Task.CompletedTask);

                    A.CallTo(() => mockIpService.PublicIp).Returns("127.0.0.1");

                    return mockIpService;
                });

                services.RemoveAll<DatabaseInitializer>();
                services.AddScoped(sp =>
                {
                    var dbContext = sp.GetRequiredService<ApplicationDbContext>();

                    var config = new ConfigurationBuilder()
                        .AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            { "InitializationDb:DropAzureTable", "false" }
                        })
                        .Build();

                    return A.Fake<DatabaseInitializer>(x => x.WithArgumentsForConstructor(
                        () => new DatabaseInitializer(
                            dbContext,
                            A.Fake<ILogger<DatabaseInitializer>>(),
                            A.Fake<RoleControllerService>(),
                            A.Fake<RoleManager<AspNetRole>>(),
                            A.Fake<UserManager<AspNetUser>>(),
                            config,
                            A.Fake<IWebHostEnvironment>()
                        )
                    ));
                });
                                
                services.AddSingleton<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>();
                services.AddSingleton<IOptionsMonitor<TestAuthHandlerOptions>>(
                    sp => new StaticOptionsMonitor<TestAuthHandlerOptions>(CurrentUserOptions));

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => {});
            });

        builder.UseEnvironment("Testing");
    }
}


