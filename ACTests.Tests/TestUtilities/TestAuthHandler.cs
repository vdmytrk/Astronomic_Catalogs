using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ACTests.Tests.TestUtilities;

public class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
{
    public const string SchemeName = "TestAuth";

    public TestAuthHandler(IOptionsMonitor<TestAuthHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder) 
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Guest simulation
        if (!Options.IsAuthenticated)
            return Task.FromResult(AuthenticateResult.Fail("Not authenticated in test"));

        // Authenticated user simulation
        var claims = new List<Claim>();

        if (!string.IsNullOrEmpty(Options.UserId))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, Options.UserId));

        if (!string.IsNullOrEmpty(Options.UserName))
            claims.Add(new Claim(ClaimTypes.Name, Options.UserName));

        claims.AddRange(Options.Claims);

        var identity = new ClaimsIdentity(
            claims,
            IdentityConstants.ApplicationScheme, 
            ClaimTypes.Name,
            ClaimTypes.Role);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
