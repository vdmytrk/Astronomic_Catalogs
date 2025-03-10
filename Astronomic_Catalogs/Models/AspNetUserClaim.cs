using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

[Table("AspNetUserClaims")]
public class AspNetUserClaim : IdentityUserClaim<string>
{
    [ForeignKey("UserId")]
    public AspNetUser User { get; set; } = null!;
}
