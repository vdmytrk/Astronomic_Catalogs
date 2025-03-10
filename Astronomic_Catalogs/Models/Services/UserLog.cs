namespace Astronomic_Catalogs.Models.Services;

public class UserLog
{
    public int Id { get; set; }
    public string IpAddress { get; set; } = string.Empty; // For clear code without squiggle.
    public string UserName { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string Referer { get; set; } = string.Empty;
    public int StatusCode { get; set; } 
    public DateTime RequestTimeUtc { get; set; }    
    public DateTime LastRequestTime { get; set; }
    public string Country { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public bool IsMobile { get; set; }  
    public string OsName { get; set; } = string.Empty;
    public string BrowserName { get; set; } = string.Empty;
    public int MaxRequests { get; set; }
    public int TimeWindowMinutes { get; set; }
    public string BlockedHeaders { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
}

