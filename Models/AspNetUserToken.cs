using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

[Table("AspNetUserTokens")]
public class AspNetUserToken : IdentityUserToken<string>
{
    // The inherited property is commented out.
    //[StringLength(450)]
    //public string UserId { get; set; } = null!;

    //[StringLength(128)]
    //public string LoginProvider { get; set; } = null!;

    //[StringLength(128)]
    //public string Name { get; set; } = null!;

    //public string Value { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public AspNetUser User { get; set; } = null!;
}
