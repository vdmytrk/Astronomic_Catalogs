using Astronomic_Catalogs.Models;
using Azure.Core;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Astronomic_Catalogs.Controllers;

[Route("Error")]
public class ErrorController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(IWebHostEnvironment env, ILogger<ErrorController> logger)
    {
        _env = env;
        _logger = logger;
    }

    [Route("")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(ErrorViewModel? errorModel)
    {
        ErrorViewModel model;
        bool isDev = _env.IsDevelopment();
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        var ex = feature?.Error;

        var tempDataPath = TempData["Path"]?.ToString();
        string path = string.IsNullOrEmpty(tempDataPath) ? feature?.Path ?? HttpContext.Request.Path : tempDataPath;

        var tempDataStackTrace = TempData["StackTrace"]?.ToString();
        string stackTrace = string.IsNullOrEmpty(tempDataStackTrace) ? ex?.StackTrace ?? string.Empty : tempDataStackTrace;

        var tempDataErrorMessage = TempData["ErrorMessage"]?.ToString();
        string message = string.IsNullOrEmpty(tempDataErrorMessage) ? ex?.Message ?? "Server error occurred." : tempDataErrorMessage;

        var requestId = TempData["RequestId"]?.ToString();
        bool alreadyLogged = TempData["IsLogged"] as bool? ?? false;

        if (!alreadyLogged)
            _logger.LogError("Unlogged exception occurred at path: {Path}.", path);

        if (!isDev)
            message =
                "An unexpected error occurred. Please try again later. " +
                "If the issue persists, contact support and provide the Request ID shown above.";


        if (errorModel!.RequestId is null) // Since ASP.NET creates a model with fields that have default values.
            model = new ErrorViewModel
            {
                RequestId = requestId,
                ErrorMessage = message,
                StackTrace = isDev ? stackTrace : null,
                Path = path,
                IsDevelopment = isDev
            };
        else
            model = errorModel;

        return View("Error", model);
    }

    /// <summary>
    /// AJAX
    /// </summary>
    [Route("Exception")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Exception(string? requestId, string? errorMessage, string? stackTrace, string? path, int? statusCode, string? source, bool isDev = false)
    {
        var model = new ErrorViewModel
        {
            RequestId = requestId ?? string.Empty,
            ErrorMessage = errorMessage ?? "An unexpected error occurred.",
            StackTrace = isDev ? stackTrace : null,
            Path = path ?? HttpContext.Request.Path,
            StatusCode = statusCode ?? 0,
            Source = source ?? string.Empty,
            IsDevelopment = isDev
        };

        return View("Error", model);
    }

    [Route("StatusCode")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult StatusCodeHandler(int code)
    {
        bool isDev = _env.IsDevelopment();
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        var ex = feature?.Error;

        var tempDataPath = TempData["Path"]?.ToString();
        string path = string.IsNullOrEmpty(tempDataPath) ? feature?.Path ?? HttpContext.Request.Path : tempDataPath;

        var tempDataStackTrace = TempData["StackTrace"]?.ToString();
        string stackTrace = string.IsNullOrEmpty(tempDataStackTrace) ? ex?.StackTrace ?? string.Empty : tempDataStackTrace;

        var tempDataErrorMessage = TempData["ErrorMessage"]?.ToString();
        string message = string.IsNullOrEmpty(tempDataErrorMessage) ? ex?.Message ?? string.Empty : tempDataErrorMessage;

        var requestId = TempData["RequestId"]?.ToString();

        if (!isDev)
            message = code switch
            {
                404 => "Page not found.",
                403 => "You do not have permission to access this page.",
                500 => "Server error occurred.",
                _ => "An errorModel occurred."
            };

        var model = new ErrorViewModel
        {
            RequestId = requestId,
            ErrorMessage = message,
            StackTrace = isDev ? stackTrace : null,
            Path = path,
            StatusCode = code,
            IsDevelopment = isDev
        };

        return View($"{code}", model);
    }

}
