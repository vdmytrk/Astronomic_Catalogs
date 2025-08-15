using ACTests.Tests.TestUtilities;
using ACTests.Tests.WebApplicationFactory;
using Astronomic_Catalogs;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Security.Claims;

namespace ACTests.Tests.Integration.Auth;

[TestFixture]
public class TestAuthHandlerIntegrationTests
{
    private TestAuthWebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

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
        _client?.Dispose();
        _factory?.Dispose();
    }

    [Test]
    public async Task Get_AdminHomeIndex_WithoutAuth_ShouldRedirectToLogin()
    {
        // Arrange  
        new TestUserBuilder(_factory)
            .WithIsAuthenticated(false)
            .Build();

        var client = CreateTestClient();

        // Act
        var response = await client.GetAsync("/Admin/HomeAdmin/Index");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Get_AdminHomeIndex_AsNonAdmin_ShouldRedirectToAccessDenied()
    {
        // Arrange: create a user without the Admin role
        new TestUserBuilder(_factory)
            .WithId("123")
            .WithName("UserWithoutAdminRole")
            .WithClaim("Email", "user@example.com")
            .WithClaim("Department", "HQ")
            .WithClaim("CanUsersAccess", "true")  
            .WithClaim(ClaimTypes.DateOfBirth, "2000")
            .WithClaim("CanRoleAccess", "true")
            .WithClaim("CanRolelModify", "true")
            .WithClaim("CanRoleWatch", "true")
            .Build();

        var client = CreateTestClient();

        // Act
        var response = await client.GetAsync("/Admin/HomeAdmin/Index");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task AuthenticatedViaTestAuthHandler_ShouldAccessProtectedPage()
    {
        // Arrange
        new TestUserBuilder(_factory)
            .WithId("456")
            .WithName("UserWithoutAdminRole")
            .WithClaim("Email", "user@example.com")
            .WithRole(RoleNames.Admin)
            .WithClaim("Department", "HQ")
            .WithClaim("CanUsersAccess", "true")
            .WithClaim(ClaimTypes.DateOfBirth, "2000")
            .WithClaim("CanRoleAccess", "true")
            .WithClaim("CanRolelModify", "true")
            .WithClaim("CanRoleWatch", "true")
            .Build();

        var client = CreateTestClient();

        // Act
        var response = await client.GetAsync("/Admin/HomeAdmin/Index");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Authenticated user should access the protected page");
    }

    [Test]
    public async Task AuthenticatedUserWithoutRequiredClaim_ShouldGetForbidden()
    {
        // Arrange: create a user without the "CanUsersAccess" claim
        new TestUserBuilder(_factory)
            .WithId("789")
            .WithName("UserWithoutAccessClaim")
            .WithClaim("Email", "user2@example.com")
            .WithRole(RoleNames.Admin)
            .WithClaim("Department", "HQ")
            .WithClaim(ClaimTypes.DateOfBirth, "2000")
            .WithClaim("CanRoleAccess", "true")
            .WithClaim("CanRolelModify", "true")
            .WithClaim("CanRoleWatch", "true")
            .Build();

        var client = CreateTestClient();

        // Act
        var response = await client.GetAsync("/Admin/HomeAdmin/Index");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden), "Authenticated user without required claim should get Forbidden");
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
