using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;

namespace Astronomic_Catalogs.Infrastructure;

public class UserLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserLoggingMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private static readonly HttpClient HttpClient = new HttpClient();

    public UserLoggingMiddleware(RequestDelegate next, ILogger<UserLoggingMiddleware> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        var osName = GetOsName(userAgent);
        var browserName = GetBrowserName(userAgent);
        var isMobile = userAgent.Contains("Mobi");
        var requstTime = DateTime.UtcNow;

        var log = new UserLog
        {
            IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            UserName = context.User.Identity?.IsAuthenticated == true ? context.User.Identity.Name ?? "Unknown" : "Anonymous",
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
            BlockedHeaders = ""
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
            _logger.LogError(ex, "Exception occurred while processing request.");
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
        try
        {
            var response = await HttpClient.GetStringAsync($"https://ipapi.co/{log.IpAddress}/json/");
            var geoData = JsonSerializer.Deserialize<JsonElement>(response);
            log.Country = geoData.GetProperty("country_name").GetString() ?? "Unknown";
            log.Region = geoData.GetProperty("region").GetString() ?? "Unknown";
            log.City = geoData.GetProperty("city").GetString() ?? "Unknown";
            log.TimeZone = geoData.GetProperty("timezone").GetString() ?? "Unknown";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get geolocation data.");
        }
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
