using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Astronomic_Catalogs.Models;

[Table("AspNetRoles")]
public class AspNetRole : IdentityRole
{
    public ICollection<AspNetRoleClaim> RoleClaims { get; set; } = new List<AspNetRoleClaim>();
    public ICollection<AspNetUserRole> UserRoles { get; set; } = new List<AspNetUserRole>();
}
