using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models.Services;
using Astronomic_Catalogs.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Astronomic_Catalogs.Infrastructure;

/// <summary>
/// This middleware logs REQUESTS to the resource.
/// </summary>
public class RequestLoggingMiddleware (RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, PublicIpService publicIpService)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;
    private readonly PublicIpService _publicIpService = publicIpService;

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        var log = new RequestLog
        {
            IpAddress = context.Items["PublicIp"]!.ToString()!,
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            RequestTime = DateTime.UtcNow,
            UserName = context.User.Identity?.IsAuthenticated == true ? context.User.Identity.Name ?? "Unknown" : "Anonymous",
            Path = context.Request.Path,
            Method = context.Request.Method,
            Referer = context.Request.Headers["Referer"].ToString()
        };

        try
        {
            await _next(context);
            log.StatusCode = context.Response.StatusCode;
        }
        catch (Exception ex)
        {
            log.StatusCode = 500;
            log.ErrorMessage = ex.Message;
            _logger.LogError(ex, "Error while saving RequestLog to the database.");
        }

        dbContext.RequestLogs.Add(log);
        await dbContext.SaveChangesAsync();
    }
}

