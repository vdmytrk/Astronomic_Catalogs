using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Astronomic_Catalogs.Models.Loging;

public class NLogLogings
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }

    [Required]
    [MaxLength(50)]
    public string Level { get; set; }

    [Required]
    [MaxLength(255)]
    public string Logger { get; set; }

    [Required]
    public string Message { get; set; }

    public string? Exception { get; set; }
}
