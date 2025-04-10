using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Astronomic_Catalogs.Infrastructure;

namespace Astronomic_Catalogs.Services;

public class JwtService
{
    private readonly UserManager<AspNetUser> _userManager;
    //private readonly AuthenticationSettingsProvider _authSettings;
    private readonly IConfiguration _configuration;

    //public JwtService(UserManager<AspNetUser> userManager, AuthenticationSettingsProvider authSettings)
    public JwtService(UserManager<AspNetUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<string> AuthenticateToken(HttpContext httpContext, AspNetUser user, bool rememberMe)
    {
        var claims = await GetUserClaims(user);
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { IsPersistent = rememberMe };

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

        return GenerateJwtToken(claims);
    }

    private async Task<List<Claim>> GetUserClaims(AspNetUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            // DV: They are used only for JWT
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    public string GenerateJwtToken(List<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]
                ?? throw new ArgumentNullException("JwtSettings:Key", "JwtSettings Key can't be null.")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            //issuer: _authSettings.JwtIssuer,
            issuer: _configuration["JwtSettings:Issuer"] 
                ?? throw new ArgumentNullException("JwtSettings:Issuer", "JwtSettings Issuer can't be null."),
            //audience: _authSettings.JwtAudience,
            audience: _configuration["JwtSettings:Audience"]
                ?? throw new ArgumentNullException("JwtSettings:Audience", "JwtSettings Audience can't be null."),
            claims: claims,
            //expires: DateTime.UtcNow.AddMinutes(_authSettings.JwtExpireMinutes),
            expires: DateTime.UtcNow.AddMinutes(int.TryParse(_configuration["JwtSettings:ExpireMinutes"], out var expireMinutes)
                ? expireMinutes
                : throw new ArgumentNullException("JwtSettings:ExpireMinutes", "JwtSettings ExpireMinutes can't be null.")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}
