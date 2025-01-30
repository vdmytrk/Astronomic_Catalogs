using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Astronomic_Catalogs.Models.Logging;


public class NLogLogging
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    [Required]
    [MaxLength(50)]
    public string Level { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string? Logger { get; set; } = string.Empty!;

    [Required]
    public string Message { get; set; } = string.Empty;

    public string? Exception { get; set; }

}
