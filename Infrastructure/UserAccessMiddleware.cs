using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Models.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json; 

namespace Astronomic_Catalogs.Infrastructure;


/// <summary>
/// This middleware restricts a registered user's access to a resource based on the administrator's actions.
/// </summary>
public class UserAccessMiddleware(
        RequestDelegate next,
        IServiceScopeFactory serviceScopeFactory)
{
    private readonly RequestDelegate _next = next;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    public async Task InvokeAsync(HttpContext context)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            bool IsAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

            if (IsAuthenticated)
            {
                var userName = context.User.Identity!.Name ?? "Unknown";
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspNetUser>>();
                var aspNetUser = await userManager.Users
                    .Where(u => u.UserName == userName)
                    .Select(u => new { u.LockoutEnabled, u.LockoutEnd })
                    .FirstOrDefaultAsync();
                bool lockoutEnd = false;
                bool lockoutEnabled = false;
                string message = string.Empty;

                if (aspNetUser != null)
                {
                    lockoutEnd = aspNetUser!.LockoutEnd > DateTime.UtcNow;
                    lockoutEnabled = aspNetUser!.LockoutEnabled == true;
                    message = $"You are denied access until {aspNetUser!.LockoutEnd.ToString()}.";
                }
                else
                {
                    message = $"You are denied access forever.";
                }

                if (lockoutEnabled && lockoutEnd)
                {
                    context.Response.StatusCode = 429;
                    await context.Response.WriteAsync(message);
                    return;
                }
            }

            await _next(context);            
        }
    }

}
