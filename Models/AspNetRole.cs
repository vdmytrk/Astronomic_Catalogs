using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

[Table("AspNetRoles")]
public class AspNetRole : IdentityRole
{
    // The inherited property is commented out.
    //[Key]
    //[StringLength(450)]
    //public string Id { get; set; } = null!;

    //[StringLength(256)]
    //public string Name { get; set; } = string.Empty;

    //[StringLength(256)]
    //public string NormalizedName { get; set; } = string.Empty;

    //public string ConcurrencyStamp { get; set; } = string.Empty;

    public ICollection<AspNetRoleClaim> RoleClaims { get; set; } = new List<AspNetRoleClaim>();
    public ICollection<AspNetUserRole> UserRoles { get; set; } = new List<AspNetUserRole>();
}
