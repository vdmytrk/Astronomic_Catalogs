using ACTests.Tests.TestUtilities;
using ACTests.Tests.WebApplicationFactory;
using Astronomic_Catalogs;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace ACTests.Tests.Integration;

[TestFixture]
public class LoginViaUIIntegrationTests : IDisposable
{
    private LoginViaUIWebApplicationFactory<Program> _factory = null!;

    [SetUp]
    public async Task Setup()
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

        var logsPath = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logsPath);

        _factory = new LoginViaUIWebApplicationFactory<Program>();

        await TestIdentitySeeder.EnsureTestAdminUserAsync(_factory.Services);
    }


    [TearDown]
    public void TearDown()
    {
        //_client?.Dispose();
        _factory?.Dispose();
    }

    public void Dispose()
    {
        TearDown();
    }


    [Test]
    public async Task Index_WithoutAuth_RedirectsToLogin()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("/Admin/HomeAdmin/Index");

        // Assert
        var location = response.Headers.Location!;
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
        Assert.That(location, Is.Not.Null);
        Assert.That(location.AbsolutePath, Does.StartWith("/Identity/Account/Login"));
    }

    [Test]
    public async Task Login_ShouldFail_WhenPasswordIsIncorrect()
    {
        // Arrange
        var client = CreateClient();
        var email = "TestAdmin@example.com";
        var incorrectPassword = "Wrong123!";

        await TestIdentitySeeder.EnsureTestAdminUserAsync(_factory.Services);

        var loginPage = await client.GetAsync("/Identity/Account/Login");
        var loginPageContent = await loginPage.Content.ReadAsStringAsync();

        var antiForgeryToken = IntegrationTestAuthHelper.ExtractAntiforgeryToken(loginPageContent);
        Assert.That(antiForgeryToken, Is.Not.Null, "Anti-forgery token not found in the login page.");

        var postData = new Dictionary<string, string>
            {
                { "Input.Email", email },
                { "Input.Password", incorrectPassword },
                { "__RequestVerificationToken", antiForgeryToken }
            };

        // Act
        var response = await client.PostAsync("/Identity/Account/Login", new FormUrlEncodedContent(postData));
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Expected HTTP 200 OK for failed login.");
        Assert.That(responseContent, Does.Contain("Invalid login attempt"), "Expected error message not found.");
    }

    [Test]
    public async Task Login_ShouldFail_WhenEmailNotConfirmed()
    {
        // Arrange
        var client = CreateClient();
        var email = "unconfirmeduser@example.com";
        var password = "TestPassword123!";

        // Creating a user with an UNconfirmed email
        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspNetUser>>();
            var user = new AspNetUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = false,
                YearOfBirth = 1995
            };

            var result = await userManager.CreateAsync(user, password);
            Assert.That(result.Succeeded, Is.True, "Failed to create unconfirmed user for the test");
        }

        var loginPageResponse = await client.GetAsync("/Identity/Account/Login");
        loginPageResponse.EnsureSuccessStatusCode();

        var loginPageHtml = await loginPageResponse.Content.ReadAsStringAsync();
        var antiForgeryToken = IntegrationTestAuthHelper.ExtractAntiforgeryToken(loginPageHtml);
        Assert.That(antiForgeryToken, Is.Not.Null, "Anti-forgery token not found in the login page.");

        var loginData = new Dictionary<string, string>
        {
            ["Input.Email"] = email,
            ["Input.Password"] = password,
            ["__RequestVerificationToken"] = antiForgeryToken
        };

        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Identity/Account/Login")
        {
            Content = new FormUrlEncodedContent(loginData),
            Headers = { Referrer = new Uri(client.BaseAddress!, "/Identity/Account/Login") }
        };

        // Act
        var response = await client.SendAsync(postRequest);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Login POST did not return OK");
        Assert.That(responseContent, Does.Contain("You need to confirm your email before logging in."), "Expected error message not found.");
    }

    [Test]
    public async Task AccessDenied_ShouldBeRedirected_WhenRoleMissing()
    {
        // Arrange
        var client = CreateClient();
        var email = "noroleuser@example.com";
        var password = "TestPassword123!";

        // Creating a user WITHOUT the Admin role
        await TestIdentitySeeder.EnsureTestUserWithoutRolesAsync(_factory.Services, email, password);
        await IntegrationTestAuthHelper.LoginAsUserAsync(client, email, password);

        // Act
        var protectedResponse = await client.GetAsync("/Admin");

        // Assert
        Assert.That(protectedResponse.StatusCode, Is.EqualTo(HttpStatusCode.Redirect), "Expected redirect for missing role");

        var redirectLocation = protectedResponse.Headers.Location?.ToString();
        Assert.That(redirectLocation, Is.Not.Null.And.Contains("/Identity/Account/AccessDenied"), "User was not redirected to AccessDenied page");
    }


    [Test]
    public async Task AuthenticatedUser_CanAccessProtectedPage()
    {
        // Arrange
        var client = CreateClient();

        // Act
        await IntegrationTestAuthHelper.LoginAsUserAsync(client, "TestAdmin@example.com", "P@ssw0rd!");
        var response = await client.GetAsync("/Admin/HomeAdmin/Index"); // GET до захищеного ресурсу

        // Assert
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Assert.That(html, Does.Contain("Hello TestAdmin!"));

    }

    private HttpClient CreateClient()
    {
        return _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
    }

}