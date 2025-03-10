using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;


[Table("AspNetUsers")]
public class AspNetUser : IdentityUser
{
    public DateTime RegistrationDate { get; set; }
    public DateTime? LastRegisterEmailSent { get; set; }
    public int CountRegisterEmailSent { get; set; }
    public int YearOfBirth { get; set; }

    public ICollection<AspNetUserClaim> UserClaims { get; set; } = new List<AspNetUserClaim>();
    public ICollection<AspNetUserRole> UserRoles { get; set; } = new List<AspNetUserRole>();
    public ICollection<AspNetUserLogin> UserLogins { get; set; } = new List<AspNetUserLogin>();
    public ICollection<AspNetUserToken> UserTokens { get; set; } = new List<AspNetUserToken>();
}
