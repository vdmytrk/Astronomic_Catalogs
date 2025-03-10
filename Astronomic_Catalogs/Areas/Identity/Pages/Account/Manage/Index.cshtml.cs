// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly SignInManager<AspNetUser> _signInManager;
        private readonly RoleManager<AspNetRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<AspNetUser> userManager,
            SignInManager<AspNetUser> signInManager,
            RoleManager<AspNetRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }


        public List<AspNetUserClaim> UserClaims { get; set; } = new();
        public List<AspNetRoleClaim> RoleClaims { get; set; } = new();
        public List<AspNetRole>? Roles { get; set; }


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        /// DV: To check if it is the local account
        /// </summary>
        public bool IsLocalAccount { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(AspNetUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            await _signInManager.RefreshSignInAsync(user);
            await LoadAsync(user);

            var logins = await _userManager.GetLoginsAsync(user);
            IsLocalAccount = logins.Count == 0;

            #region DV: Current claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                UserClaims = await _context.UserClaims
                    .Where(c => c.UserId == userId)
                    .ToListAsync();
            }

            if (user != null)
            {
                var roleNames = await _userManager.GetRolesAsync(user);
                Roles = await _roleManager.Roles
                    .Where(r => r.Name != null && roleNames.Contains(r.Name))
                    .ToListAsync();
            }

            if (Roles != null)
            {
                foreach (var role in Roles)
                {
                    var roleEntity = await _roleManager.FindByNameAsync(role.Name ?? "");
                    if (roleEntity != null)
                    {
                        var claims = await _roleManager.GetClaimsAsync(roleEntity);
                        RoleClaims.AddRange(claims.Select(c => new AspNetRoleClaim
                        {
                            RoleId = roleEntity.Id,
                            ClaimType = c.Type,
                            ClaimValue = c.Value
                        }));
                    }
                }
            }
            #endregion

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
