using ACTests.Tests.WebApplicationFactory;
using Astronomic_Catalogs;
using System.Security.Claims;

namespace ACTests.Tests.TestUtilities;

public class TestUserBuilder
{
    private readonly TestAuthWebApplicationFactory<Program> _factory;
    private readonly TestAuthHandlerOptions _options;

    public TestUserBuilder(TestAuthWebApplicationFactory<Program> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _options = _factory.CurrentUserOptions
                   ?? throw new InvalidOperationException("CurrentUserOptions is not initialized.");
    }

    public TestUserBuilder WithId(string id)
    {
        _options.UserId = id;
        return this;
    }

    public TestUserBuilder WithName(string userName)
    {
        _options.UserName = userName;
        return this;
    }

    public TestUserBuilder WithRole(string role)
    {
        _options.Claims.RemoveAll(c => c.Type == ClaimTypes.Role);
        _options.Claims.Add(new Claim(ClaimTypes.Role, role));
        return this;
    }

    public TestUserBuilder WithClaim(string type, string value)
    {
        _options.Claims.RemoveAll(c => c.Type == type);
        _options.Claims.Add(new Claim(type, value));
        return this;
    }

    public TestUserBuilder WithIsAuthenticated(bool isAuthenticated)
    {
        _options.IsAuthenticated = isAuthenticated;
        return this;
    }

    public TestAuthHandlerOptions Build() => _options;
}

