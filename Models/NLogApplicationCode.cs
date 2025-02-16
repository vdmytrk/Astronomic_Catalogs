namespace Astronomic_Catalogs.Models;

public class NLogApplicationCode
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime Logged { get; set; }
    public string? Level { get; set; }
    public string? Ip { get; set; }
    public string? MachineName { get; set; }
    public string? SessionId { get; set; }
    public string? Logger { get; set; }
    public string? Controller { get; set; }
    public string? Action { get; set; }
    public string? Method { get; set; }
    public string? Exception { get; set; }
    public string? Message { get; set; }
    public string? ActivityId { get; set; }
    public string? Scope { get; set; }
}
