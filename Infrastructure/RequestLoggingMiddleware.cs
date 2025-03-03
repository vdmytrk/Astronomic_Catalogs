using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Astronomic_Catalogs.Infrastructure;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
    {
        var log = new RequestLog
        {
            IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
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

