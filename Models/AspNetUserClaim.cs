using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

[Table("AspNetUserClaims")]
public class AspNetUserClaim : IdentityUserClaim<string>
{
    // The inherited property is commented out.
    //[Key]
    //public int Id { get; set; }

    //[Required]
    //[StringLength(450)]
    //public string UserId { get; set; } = string.Empty;

    //public string ClaimType { get; set; } = string.Empty;

    //public string ClaimValue { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public AspNetUser User { get; set; } = null!;
}
