using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

[Table("AspNetUserRoles")]
public class AspNetUserRole : IdentityUserRole<string>
{
    [ForeignKey("UserId")]
    public AspNetUser User { get; set; } = null!;

    [ForeignKey("RoleId")]
    public AspNetRole Role { get; set; } = null!;
}
