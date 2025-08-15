using ACTests.Tests.TestUtilities;
using ACTests.Tests.WebApplicationFactory;
using Astronomic_Catalogs;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Security.Claims;

namespace ACTests.Tests.Integration.Controllers;


[TestFixture]
public class UsersControllerIntegrationTests
{
    private TestAuthWebApplicationFactory<Program> _factory = null!;

    [SetUp]
    public void Setup()
    {
        Environment.SetEnvironmentVariable("AuthMessageSenderOptions:Email", "test@example.com");
        Environment.SetEnvironmentVariable("AuthMessageSenderOptions:Password", "testpassword");
        Environment.SetEnvironmentVariable("Authentication:Google:ClientId", "fake-google-client-id");
        Environment.SetEnvironmentVariable("Authentication:Google:ClientSecret", "fake-google-client-secret");
        Environment.SetEnvironmentVariable("Authentication:Microsoft:ClientId", "fake-ms-client-id");
        Environment.SetEnvironmentVariable("Authentication:Microsoft:ClientSecret", "fake-ms-client-secret");
        Environment.SetEnvironmentVariable("JwtSettings:Key", "fake-jwt-key");
        Environment.SetEnvironmentVariable("JwtSettings:Issuer", "fake-jwt-issuer");
        Environment.SetEnvironmentVariable("JwtSettings:Audience", "fake-jwt-audience");
        Environment.SetEnvironmentVariable("JwtSettings:ExpireMinutes", "60");
        Environment.SetEnvironmentVariable("AgeRestriction:MinAge", "18");

        _factory = new TestAuthWebApplicationFactory<Program>();
    }

    [TearDown]
    public void TearDown()
    {
        _factory?.Dispose();
    }

    [Test]
    public async Task AdminUser_ShouldSeeAdminPanelLink()
    {
        // Arrange
        new TestUserBuilder(_factory)
            .WithIsAuthenticated(true)
            .WithId("101112")
            .WithName("UserWithoutAdminRole")
            .WithClaim("Email", "user@example.com")
            .WithRole(RoleNames.Admin)
            .WithClaim("Department", "HQ")
            .WithClaim("CanUsersAccess", "true")
            .WithClaim(ClaimTypes.DateOfBirth, "2000")
            .WithClaim("CanRoleAccess", "true")
            .WithClaim("CanRolelModify", "true")
            .WithClaim("CanRoleWatch", "true");

        var client = CreateTestClient();

        // Act
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Admin user should access the home page");
        Assert.That(content, Does.Contain("Administration"), "Admin panel name should be visible for admin user.");

        var expectedLinks = new[] {
            "/Admin",
            "/Admin/RoleClaims",
            "/Admin/Roles",
            "/Admin/Users"
        };

        foreach (var link in expectedLinks)
        {
            Assert.That(content, Does.Contain(link), $"Admin link {link} should be visible for admin user");
        }

        Assert.That(content, Does.Not.Contain("/Identity/Account/Login"));
        Assert.That(content, Does.Not.Contain("/Identity/Account/Register"));
    }

    [Test]
    public async Task AdminUser_ShouldAccessUsersIndex()
    {
        // Arrange
        new TestUserBuilder(_factory)
            .WithIsAuthenticated(true)
            .WithId("100")
            .WithName("AdminUser")
            .WithRole(RoleNames.Admin)
            .WithClaim("Email", "admin@example.com")
            .WithClaim("Department", "HQ")
            .WithClaim("CanUsersAccess", "true")
            .WithClaim(ClaimTypes.DateOfBirth, "1980")
            .WithClaim("CanRoleAccess", "true")
            .WithClaim("CanRolelModify", "true")
            .WithClaim("CanRoleWatch", "true");

        var client = CreateTestClient();
         
        // Act
        var response = await client.GetAsync("/Admin/Users");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Admin user with all required claims should access /Admin/Users");
        Assert.That(content, Does.Contain("<h1>Users</h1>"), "Users index page should contain 'Users' heading for admin");

    }

    private HttpClient CreateTestClient()
    {
        return _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("http://localhost")
        });
    }
}
