using Astronomic_Catalogs.Models;

namespace Astronomic_Catalogs.Services;

public class RoleControllerService
{
    internal void SetData(AspNetRole inputRole, string[] selectedUsers, AspNetRole? existingRole = null)
    {
        var targetUser = existingRole ?? inputRole;

        targetUser.Name = inputRole.Name;
        targetUser.NormalizedName = inputRole.Name?.ToUpper();
        targetUser.ConcurrencyStamp = inputRole?.ConcurrencyStamp;

        var currentUserIds = targetUser.UserRoles.Select(ur => ur.UserId).ToList();
        var usersToRemove = targetUser.UserRoles.Where(ur => !selectedUsers.Contains(ur.UserId)).ToList();
        foreach (var userRole in usersToRemove)
        {
            targetUser.UserRoles.Remove(userRole);
        }
        foreach (var userId in selectedUsers)
        {
            if (!currentUserIds.Contains(userId))
            {
                targetUser.UserRoles.Add(new AspNetUserRole { RoleId = targetUser.Id, UserId = userId });
            }
        }
    }
}
