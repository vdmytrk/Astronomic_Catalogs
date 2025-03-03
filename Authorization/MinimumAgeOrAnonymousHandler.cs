using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Astronomic_Catalogs.Authorization;

public class MinimumAgeOrAnonymousHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        var identity = context.User.Identity;
        if (identity == null || !identity.IsAuthenticated)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var birthDateClaim = context.User.FindFirst(ClaimTypes.DateOfBirth);
        if (birthDateClaim == null || !DateTime.TryParse(birthDateClaim.Value, out DateTime birthYear))
            return Task.CompletedTask;

        int age = DateTime.Today.Year - birthYear.Year;
        if (age >= requirement.MinimumAge)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
