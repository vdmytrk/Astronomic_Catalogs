using Astronomic_Catalogs.Infrastructure;
using Astronomic_Catalogs.Services.Interfaces;
using System.Net.Http;

namespace Astronomic_Catalogs.Services;

public class PublicIpService (HttpClient httpClient, ILogger<PublicIpService> logger) : IPublicIpService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<PublicIpService> _logger = logger;
    public string PublicIp { get; set; } = string.Empty;

    public async Task GetPublicIpAsync(HttpContext context, string ip)
    {
        if (context.Items["PublicIp"] is null && (ip == "::1" || ip == "127.0.0.1"))
        {
            try
            {
                context.Items["PublicIp"] = PublicIp = await _httpClient.GetStringAsync("https://api64.ipify.org");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching public IP");
                context.Items["PublicIp"] = PublicIp =  "Unknown_ip";
            }
        }
        else
        {
            context.Items["PublicIp"] = PublicIp = ip;
        }
    }
}
