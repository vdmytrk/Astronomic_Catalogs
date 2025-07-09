namespace Astronomic_Catalogs.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
    public int? StatusCode { get; set; }
    public string? Path { get; set; }
    public string? Source { get; set; }
    public bool IsDevelopment { get; set; }
}
