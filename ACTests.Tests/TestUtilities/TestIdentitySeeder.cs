using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

internal static class TestIdentitySeeder
{
    internal static async Task EnsureTestAdminUserAsync(
        IServiceProvider services,
        string? adminEmail = null,
        string? adminPassword = null,
        int? adminYearOfBirth = null,
        bool isConfirmd = true)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AspNetRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspNetUser>>();

        string adminEmailReg = adminEmail ?? "TestAdmin@example.com";
        string adminPasswordReg = adminPassword ?? "P@ssw0rd!";
        int adminYearOfBirthReg = adminYearOfBirth ?? 2000;

        string[] roles = RoleNames.AllRoles;

        // Add roles
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var newRole = new AspNetRole { Name = roleName, NormalizedName = roleName.ToUpperInvariant() };
                await roleManager.CreateAsync(newRole);
            }
        }

        // Add or update user
        var existingUser = await userManager.FindByEmailAsync(adminEmailReg);
        if (existingUser == null)
        {
            var user = new AspNetUser
            {
                UserName = adminEmailReg,
                Email = adminEmailReg,
                EmailConfirmed = isConfirmd,
                YearOfBirth = adminYearOfBirthReg
            };

            var result = await userManager.CreateAsync(user, adminPasswordReg);
            if (!result.Succeeded)
                throw new Exception("Failed to create user: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));

            existingUser = user;
        }

        // Add roles to current user
        if (!await userManager.IsInRoleAsync(existingUser, RoleNames.Admin))
        {
            await userManager.AddToRoleAsync(existingUser, RoleNames.Admin);
        }

        // Add claims to user
        var existingClaims = await userManager.GetClaimsAsync(existingUser);
        var userClaims = new List<Claim>
        {
            new Claim("Department", "HQ"),
            new Claim("CanUsersAccess", "true"),
            new Claim(ClaimTypes.DateOfBirth, adminYearOfBirthReg.ToString())
        };

        var newUserClaims = userClaims.Where(c => !existingClaims.Any(ec => ec.Type == c.Type)).ToList();
        if (newUserClaims.Any())
        {
            await userManager.AddClaimsAsync(existingUser, newUserClaims);
        }

        // Adding claims to roles
        var roleClaims = new List<Claim>
        {
            new Claim("CanRoleAccess", "true"),
            new Claim("CanRolelModify", "true"),
            new Claim("CanRoleWatch", "true"),
        };

        foreach (var roleName in roles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null) continue;

            var existingRoleClaims = await roleManager.GetClaimsAsync(role);
            var newClaims = roleClaims.Where(c => !existingRoleClaims.Any(ec => ec.Type == c.Type)).ToList();

            foreach (var claim in newClaims)
            {
                await roleManager.AddClaimAsync(role, claim);
            }
        }
    }


    internal static async Task EnsureTestUserWithoutRolesAsync(
        IServiceProvider services,
        string email,
        string password,
        int yearOfBirth = 1990,
        bool isConfirmed = true)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspNetUser>>();

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
            var user = new AspNetUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = isConfirmed,
                YearOfBirth = yearOfBirth
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception("Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

}
