using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Astronomic_Catalogs.Authorization;

public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }
    public MinimumAgeRequirement(int minimumAge) => MinimumAge = minimumAge;
}

public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        var birthDateClaim = context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth);
        if (birthDateClaim == null)
            return Task.CompletedTask;

        int birthDate = int.Parse(birthDateClaim.Value);
        var age = DateTime.Today.Year - birthDate;
        if (age >= requirement.MinimumAge)
            context.Succeed(requirement);

        return Task.CompletedTask;

    }
}
