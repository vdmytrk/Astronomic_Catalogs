using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

[Table("AspNetRoleClaims")]
public class AspNetRoleClaim : IdentityRoleClaim<string>
{
    [ForeignKey("RoleId")]
    public AspNetRole Role { get; set; } = null!;
}
