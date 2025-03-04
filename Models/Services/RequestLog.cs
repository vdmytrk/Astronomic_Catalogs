using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models.Services;


public class RequestLog
{
    public int Id { get; set; }
    public string IpAddress { get; set; } = string.Empty; // For clear code without squiggle.
    public string UserAgent { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime RequestTime { get; set; }
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Referer { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
