using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace Astronomic_Catalogs.Utils;

static class ControllerExtensions
{
    public static async Task<string> RenderViewAsync(this Controller controller, string viewName, object? model, bool partial = false)
    {
        if (string.IsNullOrEmpty(viewName))
            viewName = controller.ControllerContext.ActionDescriptor.ActionName;

        controller.ViewData.Model = model;

        using var writer = new StringWriter();

        var viewEngine = controller.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
        var viewResult = viewEngine.FindView(controller.ControllerContext, viewName, isMainPage: !partial);

        if (!viewResult.Success)
        {
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

        await viewResult.View.RenderAsync(viewContext);
        return writer.GetStringBuilder().ToString();
    }
}
