using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

[Table("AspNetUserRoles")]
public class AspNetUserRole : IdentityUserRole<string>
{
    // The inherited property is commented out.
    //[StringLength(450)]
    //public string UserId { get; set; } = null!;

    //[StringLength(450)]
    //public string RoleId { get; set; } = null!;

    [ForeignKey("UserId")]
    public AspNetUser User { get; set; } = null!;

    [ForeignKey("RoleId")]
    public AspNetRole Role { get; set; } = null!;
}
