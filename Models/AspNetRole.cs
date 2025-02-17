using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

[Table("AspNetRoles")]
public class AspNetRole : IdentityRole
{
    public ICollection<AspNetRoleClaim> RoleClaims { get; set; } = new List<AspNetRoleClaim>();
    public ICollection<AspNetUserRole> UserRoles { get; set; } = new List<AspNetUserRole>();
}
