using Astronomic_Catalogs.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;

namespace Astronomic_Catalogs.Utils;

static class ControllerExtensions
{
    public static async Task<string> RenderViewAsync(this Controller controller, string viewName, object? model, bool partial = false)
    {
        var logger = controller.HttpContext.RequestServices.GetService<ILoggerFactory>()?
            .CreateLogger(controller.GetType());
        var requestId = Activity.Current?.Id ?? controller.HttpContext.TraceIdentifier;


        if (string.IsNullOrEmpty(viewName))
            viewName = controller.ControllerContext.ActionDescriptor.ActionName;

        controller.ViewData.Model = model;

        using var writer = new StringWriter();

        var viewEngine = controller.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
        var viewResult = viewEngine.FindView(controller.ControllerContext, viewName, isMainPage: !partial);

        if (!viewResult.Success)
        {

            logger?.LogError("View '{ViewName}' not found. RequestId: {RequestId}. Searched locations:\n{Locations}",
                viewName, requestId, string.Join("\n", viewResult.SearchedLocations));

            throw new FileNotFoundException($"View '{viewName}' not found. Searched locations:\n" +
                string.Join("\n", viewResult.SearchedLocations));
        }

        var viewContext = new ViewContext(
            controller.ControllerContext,
            viewResult.View,
            controller.ViewData,
            controller.TempData,
            writer,
            new HtmlHelperOptions()
        );


        if (partial)
        {
            viewContext.ExecutingFilePath = viewName;
            controller.ViewData["Layout"] = null; // Disable layout to prevent a partial view from wrapping.
        }

        try
        {
            await viewResult.View.RenderAsync(viewContext);
            return writer.GetStringBuilder().ToString();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Rendering error for view '{ViewName}' RequestId: {RequestId}", viewName, requestId);
            throw new ViewRenderingException($"Failed to render view '{viewName}'. INNER ERROR: {ex.Message} RequestId: {requestId}", ex);
        }
    }
}
