namespace Astronomic_Catalogs.Infrastructure.Configuration;

static class ConfigKeys
{
    public const string AuthEmail = "AuthMessageSenderOptions-Email";
    public const string AuthPassword = "AuthMessageSenderOptions-Password";

    public const string GoogleClientId = "Authentication-Google-ClientId";
    public const string GoogleClientSecret = "Authentication-Google-ClientSecret";

    public const string MicrosoftClientId = "Authentication-Microsoft-ClientId";
    public const string MicrosoftClientSecret = "Authentication-Microsoft-ClientSecret";

    public const string JwtKey = "JwtSettings-Key";
    public const string JwtIssuer = "JwtSettings-Issuer";
    public const string JwtAudience = "JwtSettings-Audience";
    public const string JwtExpireMinutes = "JwtSettings-ExpireMinutes";

    public const string MinAge = "AgeRestriction-MinAge";
}

