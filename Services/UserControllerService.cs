using Astronomic_Catalogs.Models;

namespace Astronomic_Catalogs.Services;

public class UserControllerService
{
    internal void SetData(AspNetUser inputUser, string[] selectedRoles, AspNetUser? existingUser = null)
    {
        var targetUser = existingUser ?? inputUser;
        var currentRoleIds = targetUser.UserRoles.Select(ur => ur.RoleId).ToList();
        var rolesToRemove = targetUser.UserRoles.Where(ur => !selectedRoles.Contains(ur.RoleId)).ToList();

        targetUser.UserName = inputUser.UserName;
        targetUser.Email = inputUser.Email;
        targetUser.EmailConfirmed = inputUser.EmailConfirmed;

        // Security fields.
        // Updating SecurityStamp when changing critical data (password, email).
        if (targetUser.Email != inputUser.Email || targetUser.PasswordHash != inputUser.PasswordHash)
        {
            targetUser.SecurityStamp = Guid.NewGuid().ToString();
        }
        // Updating ConcurrencyStamp to resolve update conflicts.
        targetUser.ConcurrencyStamp = Guid.NewGuid().ToString();

        targetUser.PhoneNumber = inputUser.PhoneNumber;
        targetUser.PhoneNumberConfirmed = inputUser.PhoneNumberConfirmed;

        targetUser.YearOfBirth = inputUser.YearOfBirth;
        targetUser.TwoFactorEnabled = inputUser.TwoFactorEnabled;
        targetUser.LockoutEnd = inputUser.LockoutEnd;
        targetUser.LockoutEnabled = inputUser.LockoutEnabled;
        targetUser.AccessFailedCount = inputUser.AccessFailedCount;

        foreach (var role in rolesToRemove)
        {
            targetUser.UserRoles.Remove(role);
        }
        foreach (var roleId in selectedRoles)
        {
            if (!currentRoleIds.Contains(roleId))
            {
                targetUser.UserRoles.Add(new AspNetUserRole { RoleId = roleId, UserId = inputUser.Id });
            }
        }
    }
}
