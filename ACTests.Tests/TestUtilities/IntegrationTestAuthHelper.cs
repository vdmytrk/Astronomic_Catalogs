using System.Net;
using System.Text.RegularExpressions;

namespace ACTests.Tests.TestUtilities;

internal static class IntegrationTestAuthHelper
{
    internal static async Task LoginAsUserAsync(HttpClient client, string email, string password)
    {
        // 1. GET login page to extract antiforgery token
        var loginPageResponse = await client.GetAsync("/Identity/Account/Login");
        loginPageResponse.EnsureSuccessStatusCode();

        var loginPageHtml = await loginPageResponse.Content.ReadAsStringAsync();
        var token = ExtractAntiforgeryToken(loginPageHtml);

        // 2. Prepare form content
        var loginData = new Dictionary<string, string>
        {
            ["Input.Email"] = email,
            ["Input.Password"] = password,
            ["Input.RememberMe"] = "false",
            ["__RequestVerificationToken"] = token
        };
        var content = new FormUrlEncodedContent(loginData);

        // 3. POST login
        var response = await client.PostAsync("/Identity/Account/Login", content);

        if (response.StatusCode != HttpStatusCode.Redirect) 
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
               $"Login failed with status code {response.StatusCode}.\n" +
               $"Response content: {responseBody}");
        }
    }

    internal static string ExtractAntiforgeryToken(string html)
    {
        var match = Regex.Match(html, @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
        if (!match.Success)
            throw new Exception("Antiforgery token not found in login form.");
        return match.Groups[1].Value;
    }
}

