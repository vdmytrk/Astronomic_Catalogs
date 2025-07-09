using Astronomic_Catalogs.Services.Interfaces;
using System.Net;

namespace Astronomic_Catalogs.Services;

public class ExceptionRedirectUrlService : IExceptionRedirectUrlService
{
    private readonly IWebHostEnvironment _env;

    public ExceptionRedirectUrlService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public string BuildRedirectUrl(Exception ex, string requestId, string? path = null)
    {
        var errorMessage = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred.";
        var stackTrace = _env.IsDevelopment() ? ex.StackTrace : null;
        var source = ex.Source ?? "Unknown";

        var errorUrl = $"/Error/Exception" +
                       $"?requestId={Uri.EscapeDataString(requestId)}" +
                       $"&errorMessage={Uri.EscapeDataString(errorMessage)}" +
                       $"&stackTrace={Uri.EscapeDataString(stackTrace ?? "")}" +
                       $"&path={Uri.EscapeDataString(path ?? "N/A")}" +
                       $"&statusCode={(int)HttpStatusCode.InternalServerError}" +
                       $"&source={Uri.EscapeDataString(source)}" +
                       $"&isDev={_env.IsDevelopment()}";

        return errorUrl;
    }
}

