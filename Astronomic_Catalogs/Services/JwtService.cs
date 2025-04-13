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
    private readonly AuthenticationSettingsProvider _authSettings;

    public JwtService(UserManager<AspNetUser> userManager, AuthenticationSettingsProvider authSettings)
    {
        _userManager = userManager;
        _authSettings = authSettings;
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
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.JwtKey
                ?? throw new ArgumentNullException("JwtSettings:Key", "JwtSettings Key can't be null.")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var addMinute = _authSettings.JwtExpireMinutes == 0 ? 10 : _authSettings.JwtExpireMinutes;

        var token = new JwtSecurityToken(
            issuer: _authSettings.JwtIssuer 
                ?? throw new ArgumentNullException("JwtSettings:Issuer", "JwtSettings Issuer can't be null."),
            audience: _authSettings.JwtAudience
                ?? throw new ArgumentNullException("JwtSettings:Audience", "JwtSettings Audience can't be null."),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(addMinute),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}
