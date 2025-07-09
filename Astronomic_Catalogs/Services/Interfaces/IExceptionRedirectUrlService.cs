namespace Astronomic_Catalogs.Services.Interfaces;

public interface IExceptionRedirectUrlService
{
    string BuildRedirectUrl(Exception ex, string requestId, string? path = null);
}
