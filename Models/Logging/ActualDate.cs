using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models.Logging;


public class ActualDate
{
    public int? Id { get; set; }
    public DateTime ActualDateProperty { get; set; }
}
