using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.SignalR;

namespace Astronomic_Catalogs.Services;

public class ProgressHub : Hub
{
    public async Task SendProgress(string jobId, int percent)
    {
        await Clients.Group(jobId).SendAsync("ReceiveProgress", percent);
    }

    public override async Task OnConnectedAsync()
    {
        var jobId = Context.GetHttpContext()?.Request.Query["jobId"];
        if (!string.IsNullOrEmpty(jobId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, jobId!);
        }
        await base.OnConnectedAsync();
    }
}
