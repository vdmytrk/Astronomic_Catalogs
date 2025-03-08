using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Models.Configuration.Services;
using Astronomic_Catalogs.Models.Services;
using Astronomic_Catalogs.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace Astronomic_Catalogs.Infrastructure;

/// <summary>
/// This middleware logs USERS who access the resource and restricts access to the resource for an IP address based on 
///     the actions of the database administrator.
/// </summary>
public class UserLoggingMiddleware(
        RequestDelegate next,
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<UserLoggingMiddleware> logger,
        IServiceScopeFactory serviceScopeFactory)
{
    private readonly RequestDelegate _next = next;
    private readonly HttpClient _httpClient = httpClient;
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<UserLoggingMiddleware> _logger = logger;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;


    public async Task InvokeAsync(HttpContext context)
    {
        string userAgent = context.Request.Headers["User-Agent"].ToString();
        var osName = GetOsName(userAgent);
        var browserName = GetBrowserName(userAgent);
        var isMobile = userAgent.Contains("Mobi");
        var requstTime = DateTime.UtcNow;
        bool IsAuthenticated = context.User.Identity?.IsAuthenticated ?? false;


        var log = new UserLog
        {
            IpAddress = context.Items["PublicIp"]!.ToString()!,
            UserName = IsAuthenticated == true ? context.User.Identity!.Name ?? "Unknown" : "Anonymous",
            UserAgent = userAgent,
            Route = context.Request.Path,
            HttpMethod = context.Request.Method,
            Referer = context.Request.Headers["Referer"].ToString(),
            RequestTimeUtc = requstTime,
            LastRequestTime = requstTime,
            StatusCode = 0,
            ErrorMessage = "",
            OsName = osName,
            BrowserName = browserName,
            IsMobile = isMobile,
            Country = "Unknown",
            Region = "Unknown",
            City = "Unknown",
            TimeZone = "Unknown",
            MaxRequests = 10000,
            TimeWindowMinutes = 5,
            BlockedHeaders = "",
            Latitude = "Unknown",
            Longitude = "Unknown"
        };

        await SetGeoLocationData(log);

        if (await IsRequestBlocked(log))
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync("Too Many Requests");
            return;
        }

        try
        {
            await _next(context);
            log.StatusCode = context.Response.StatusCode;
        }
        catch (Exception ex)
        {
            log.StatusCode = 500;
            log.ErrorMessage = ex.Message;
            throw;
        }

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.UserLogs.Add(log);
            await dbContext.SaveChangesAsync();
        }
    }



    private async Task SetGeoLocationData(UserLog log)
    {
        if (string.IsNullOrEmpty(log.IpAddress)) return;

        if (_cache.TryGetValue(log.IpAddress, out GeoLocationData? cachedGeo))
        {
            if (cachedGeo != null)
            {
                log.Country = cachedGeo.Country;
                log.Region = cachedGeo.Region;
                log.City = cachedGeo.City;
                log.TimeZone = cachedGeo.TimeZone;
                log.Latitude = cachedGeo.Latitude.ToString();
                log.Longitude = cachedGeo.Longitude.ToString();
                return;
            }
        }

        try
        {
            var response = await _httpClient.GetStringAsync($"https://ipapi.co/{log.IpAddress}/json/");
            var geoData = JsonSerializer.Deserialize<JsonElement>(response);

            var newGeoData = new GeoLocationData
            {
                Country = GetPropertyIfExists(geoData, "country_name"),
                Region = GetPropertyIfExists(geoData, "region"),
                City = GetPropertyIfExists(geoData, "city"),
                TimeZone = GetPropertyIfExists(geoData, "timezone"),
                Latitude = GetPropertyIfExists(geoData, "latitude"),
                Longitude = GetPropertyIfExists(geoData, "longitude")
            };

            _cache.Set(log.IpAddress, newGeoData, TimeSpan.FromDays(1));

            log.Country = newGeoData.Country;
            log.Region = newGeoData.Region;
            log.City = newGeoData.City;
            log.TimeZone = newGeoData.TimeZone;
            log.Latitude = newGeoData.Latitude.ToString();
            log.Longitude = newGeoData.Longitude.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get geolocation data.");
        }
    }

    private static string GetPropertyIfExists(JsonElement geoData, string propertyName, string defaultValue = "Unknown")
    {
        if (geoData.TryGetProperty(propertyName, out var jsonProperty))
        {
            return jsonProperty.ValueKind switch
            {
                JsonValueKind.String => jsonProperty.GetString() ?? defaultValue,
                JsonValueKind.Number => jsonProperty.GetRawText(), // Зберігаємо число у вигляді рядка
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => defaultValue
            };
        }

        return defaultValue;
    }


    private async Task<bool> IsRequestBlocked(UserLog log)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var recentRequests = await dbContext.UserLogs
                .Where(ul => ul.IpAddress == log.IpAddress && ul.RequestTimeUtc > DateTime.UtcNow.AddMinutes(-log.TimeWindowMinutes))
                .CountAsync();
            int maxRequests = await dbContext.UserLogs
                .Where(ul => ul.IpAddress == log.IpAddress)
                .OrderBy(ul => ul.RequestTimeUtc)
                .Select(ul => ul.MaxRequests)
                .FirstOrDefaultAsync();

            return recentRequests > maxRequests;
        }
    }

    private string GetOsName(string userAgent)
    {
        if (userAgent.Contains("Windows")) return "Windows";
        if (userAgent.Contains("Mac")) return "MacOS";
        if (userAgent.Contains("Linux")) return "Linux";
        return "Unknown";
    }

    private string GetBrowserName(string userAgent)
    {
        if (userAgent.Contains("Firefox")) return "Firefox";
        if (userAgent.Contains("Chrome")) return "Chrome";
        if (userAgent.Contains("Safari")) return "Safari";
        if (userAgent.Contains("Edge")) return "Edge";
        return "Unknown";
    }
}
