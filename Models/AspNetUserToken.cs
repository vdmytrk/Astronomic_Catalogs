using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

[Table("AspNetUserTokens")]
public class AspNetUserToken : IdentityUserToken<string>
{
    [ForeignKey("UserId")]
    public AspNetUser User { get; set; } = null!;
}
