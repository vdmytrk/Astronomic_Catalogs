// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Astronomic_Catalogs.Services;

namespace Astronomic_Catalogs.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<AspNetUser> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool DisplayConfirmAccountLink { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string EmailConfirmationUrl { get; set; }

        [BindProperty]
        public string ResendEmail { get; set; } = null!; // DV: To retrieve a value from the form for resending the email.

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }
            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;

            /// Once you add a real email sender, you should remove this code that lets you confirm the account
            ///DisplayConfirmAccountLink = true;
            ///if (DisplayConfirmAccountLink)
            ///{
            ///    var userId = await _userManager.GetUserIdAsync(user);
            ///    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            ///    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            ///    EmailConfirmationUrl = Url.Page(
            ///        "/Account/ConfirmEmail",
            ///        pageHandler: null,
            ///        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
            ///        protocol: Request.Scheme);
            ///}

            DisplayConfirmAccountLink = false;

            // DV: 
            if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId, code },
                    protocol: Request.Scheme);
            }

            return Page();
        }


        /// <summary>
        /// TODO: Set time before send new letter.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(string resendEmail)
        {
            if (string.IsNullOrEmpty(resendEmail))
            {
                ModelState.AddModelError(string.Empty, "Email is required.");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(resendEmail);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{resendEmail}'.");
            }


            // TODO: Check for correct registration.
if (user.EmailConfirmed)
{
    return RedirectToPage("Login");
}

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = user.Id, code },
            protocol: Request.Scheme);

            await _sender.SendEmailAsync(
                resendEmail,
                "Confirm your email",
                $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");

            return Page();
        }
    }
}
