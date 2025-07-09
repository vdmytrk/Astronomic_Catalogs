namespace Astronomic_Catalogs.Exceptions;

public class ViewRenderingException : Exception
{
    public ViewRenderingException(string message, Exception? inner = null)
        : base(message, inner) { }
}
