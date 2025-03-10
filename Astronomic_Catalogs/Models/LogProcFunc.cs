namespace Astronomic_Catalogs.Models;

public class LogProcFunc
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public string? FuncProc { get; set; }
    public int Line { get; set; }
    public int ErrorNumber { get; set; }
    public int? ErrorSeverity { get; set; }
    public int? ErrorState { get; set; }
    public string ErrorMessage { get; set; } = string.Empty; // For clear code without squiggle.
}
