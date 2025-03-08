using Astronomic_Catalogs.Infrastructure;
using System.Net.Http;

namespace Astronomic_Catalogs.Services;

public class PublicIpService (HttpClient httpClient, ILogger<UserLoggingMiddleware> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<UserLoggingMiddleware> _logger = logger;
    public string PublicIp { get; set; } = string.Empty;

    public async Task GetPublicIpAsync(HttpContext context, string ip)
    {
        if (ip == "::1" || ip == "127.0.0.1")
        {
            try
            {
                context.Items["PublicIp"] = PublicIp = await _httpClient.GetStringAsync("https://api64.ipify.org");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching public IP");
                PublicIp =  "Unknown_ip";
            }
        }
        else
        {
            context.Items["PublicIp"] = PublicIp = ip;
        }
    }
}
