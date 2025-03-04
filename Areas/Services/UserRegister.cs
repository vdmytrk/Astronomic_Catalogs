using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Astronomic_Catalogs.Areas.Services;

public class UserRegister (UserManager<AspNetUser> userManager, RoleManager<AspNetRole> roleManager)
{
    private readonly UserManager<AspNetUser> _userManager = userManager;
    private readonly RoleManager<AspNetRole> _roleManager = roleManager;
        
    internal async Task AddUserClaims(string adminEmail, int adminYearOfBirth)
    {
        var existUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
        if (existUser != null)
        {
            var existingClaims = await _userManager.GetClaimsAsync(existUser);
            var claimsToAdd = new List<Claim>
                    {
                        new Claim("CanUsersAccess", "true"),
                        new Claim(ClaimTypes.DateOfBirth, adminYearOfBirth.ToString())
                    };

            var newClaims = claimsToAdd.Where(c => !existingClaims.Any(ec => ec.Type == c.Type)).ToList();
            if (newClaims.Any())
                await _userManager.AddClaimsAsync(existUser, newClaims);
        }
    }
    internal async Task AddRoleClaims()
    {
        string[] roles = RoleNames.AllRoles;
        var claimsToAdd = new List<Claim>
        {
            new Claim("CanRoleAccess", "true"),
            new Claim("CanRolelModify", "true"),
            new Claim("CanRoleWatch", "true"),
        };

        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null)
                throw new Exception($"Adding a claim to the roleName is not possible because the {roleName} role does not exist.");

            var existingClaims = await _roleManager.GetClaimsAsync(role);
            var newClaims = claimsToAdd.Where(c => !existingClaims.Any(ec => ec.Type == c.Type)).ToList();
            foreach (var claim in newClaims)
            {
                await _roleManager.AddClaimAsync(role, claim);
            }
        }
    }
}
