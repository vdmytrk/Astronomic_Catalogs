using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models.Services;
using Astronomic_Catalogs.Services;

namespace Astronomic_Catalogs.Infrastructure;

/// <summary>
/// This middleware logs REQUESTS to the resource.
/// </summary>
public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
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

        await _next(context);
        log.StatusCode = context.Response.StatusCode;

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.RequestLogs.Add(log);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception dbEx)
        {
            _logger.LogError(dbEx, "Error while saving request log.");
        }

    }

}

