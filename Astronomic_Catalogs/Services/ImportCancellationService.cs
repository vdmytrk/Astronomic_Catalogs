using Astronomic_Catalogs.Services.Interfaces;
using System.Collections.Concurrent;

namespace Astronomic_Catalogs.Services;

public class ImportCancellationService : IImportCancellationService
{
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _tokens = new();

    public CancellationTokenSource GetOrCreateToken(string jobId)
    {
        return _tokens.GetOrAdd(jobId, _ => new CancellationTokenSource());
    }

    public void Cancel(string jobId)
    {
        if (_tokens.TryRemove(jobId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    public void Remove(string jobId)
    {
        if (_tokens.TryRemove(jobId, out var cts))
        {
            cts.Dispose();
        }
    }
}
