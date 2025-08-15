// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Astronomic_Catalogs.Areas.Identity.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<AspNetUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;
    private readonly UserManager<AspNetUser> _userManager;
    private readonly JwtService _jwtService;


    public LoginModel(
        SignInManager<AspNetUser> signInManager,
        ILogger<LoginModel> logger,
        UserManager<AspNetUser> userManager,
        JwtService jwtService
        )
    {
        _signInManager = signInManager;
        _logger = logger;
        _userManager = userManager;
        _jwtService = jwtService;
    }

    [BindProperty]
    public InputModel Input { get; set; }
    public IList<AuthenticationScheme> ExternalLogins { get; set; }
    public string ReturnUrl { get; set; }
    [TempData]
    public string ErrorMessage { get; set; }
    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl ??= Url.Content("~/");

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (ModelState.IsValid)
        {
            // DV: Checking if user exist
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                _logger.LogWarning($"User with {Input.Email} email not found.");
                ModelState.AddModelError(string.Empty, $"User with {Input.Email} email not found.");
                return Page();
            }

            // DV: Checking if email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogWarning($"User with email {Input.Email} has not confirmed their email.");
                ModelState.AddModelError(string.Empty, "You need to confirm your email before logging in.");
                return Page();
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true); // DV: It was false
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");

                // DV: If this is an API request (JS Fetch/AJAX), return JSON. In a future Mobile App
                if (HttpContext.Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    var authResult = await _jwtService.AuthenticateToken(HttpContext, user, Input.RememberMe);
                    return new JsonResult(new { token = authResult });
                }

                return LocalRedirect(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }

        // If we got this far, something failed, redisplay form
        return Page();
    }
}
