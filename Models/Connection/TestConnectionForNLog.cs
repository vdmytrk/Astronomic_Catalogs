namespace Astronomic_Catalogs.Models.Connection;

public class TestConnectionForNLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = null!;
    public string Logger { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? Exception { get; set; }
}
