namespace Astronomic_Catalogs.Services.Interfaces;

public interface IImportCancellationService
{
    CancellationTokenSource GetOrCreateToken(string jobId);
    void Cancel(string jobId);
    void Remove(string jobId);
}
