using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ACTests.Tests.TestUtilities;

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public List<Claim> Claims { get; set; } = new();
    public bool IsAuthenticated { get; set; } = true;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
}
