namespace Astronomic_Catalogs.Services.Interfaces;

public interface IPublicIpService
{
    Task GetPublicIpAsync(HttpContext context, string ip);
    string PublicIp { get; set; }
}
