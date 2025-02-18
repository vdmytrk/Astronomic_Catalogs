using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

[Table("AspNetRoleClaims")]
public class AspNetRoleClaim : IdentityRoleClaim<string>
{
    // The inherited property is commented out.
    //[Key]
    //public int Id { get; set; }

    //[Required]
    //[StringLength(450)]
    //public string RoleId { get; set; } = string.Empty;

    //public string ClaimType { get; set; } = string.Empty;

    //public string ClaimValue { get; set; } = string.Empty;

    [ForeignKey("RoleId")]
    public AspNetRole Role { get; set; } = null!;
}
