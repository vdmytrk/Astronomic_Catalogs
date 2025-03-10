using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;

[Table("AspNetUserLogins")]
public class AspNetUserLogin : IdentityUserLogin<string>
{
    [ForeignKey("UserId")]
    public AspNetUser User { get; set; } = null!;
}
