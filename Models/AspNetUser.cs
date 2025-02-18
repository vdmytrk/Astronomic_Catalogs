using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astronomic_Catalogs.Models;


[Table("AspNetUsers")]
public class AspNetUser : IdentityUser
{
    // The inherited property is commented out.
    //[Key]
    //[StringLength(450)]
    //public string Id { get; set; } = null!;

    //[StringLength(256)]
    //public string UserName { get; set; } = string.Empty;

    //[StringLength(256)]
    //public string NormalizedUserName { get; set; } = string.Empty;

    //[StringLength(256)]
    //public string Email { get; set; } = string.Empty;

    //[StringLength(256)]
    //public string NormalizedEmail { get; set; } = string.Empty;

    //public bool EmailConfirmed { get; set; }

    //public string PasswordHash { get; set; } = string.Empty;

    //public string SecurityStamp { get; set; } = string.Empty;

    //public string ConcurrencyStamp { get; set; } = string.Empty;

    //public string PhoneNumber { get; set; } = string.Empty;

    //public bool PhoneNumberConfirmed { get; set; }

    //public bool TwoFactorEnabled { get; set; }

    //public DateTimeOffset? LockoutEnd { get; set; }

    //public bool LockoutEnabled { get; set; }

    //public int AccessFailedCount { get; set; }

    public ICollection<AspNetUserClaim> UserClaims { get; set; } = new List<AspNetUserClaim>();
    public ICollection<AspNetUserRole> UserRoles { get; set; } = new List<AspNetUserRole>();
    public ICollection<AspNetUserLogin> UserLogins { get; set; } = new List<AspNetUserLogin>();
    public ICollection<AspNetUserToken> UserTokens { get; set; } = new List<AspNetUserToken>();
}
