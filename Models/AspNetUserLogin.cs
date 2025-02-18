using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

[Table("AspNetUserLogins")]
public class AspNetUserLogin : IdentityUserLogin<string>
{
    // The inherited property is commented out.
    //[StringLength(128)]
    //public string LoginProvider { get; set; } = null!;

    //[StringLength(128)]
    //public string ProviderKey { get; set; } = string.Empty;

    //public string ProviderDisplayName { get; set; } = string.Empty;

    //[Required]
    //[StringLength(450)]
    //public string UserId { get; set; } = null!;

    [ForeignKey("UserId")]
    public AspNetUser User { get; set; } = null!;
}
