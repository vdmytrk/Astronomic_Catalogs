using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

public class DatabaseInitialization
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; } = 1;
    public bool Is_SourceType_Executed { get; set; }
    public bool Is_NGC2000_UKTemporarilySource_Executed { get; set; }
    public bool Is_NameObject_Executed { get; set; }
    public bool Is_Constellation_Executed { get; set; }
    public bool Is_NGC2000_UKTemporarily_Executed { get; set; }
    public bool Is_CollinderCatalog_Temporarily_Executed { get; set; }
    public bool Is_NGCWikipedia_TemporarilySource_Executed { get; set; }
    public bool Is_NGCWikipedia_ExtensionTemporarilySource_Executed { get; set; }
    public bool Is_NGCICOpendatasoft_Source_Executed { get; set; }
}
