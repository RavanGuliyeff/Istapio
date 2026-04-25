namespace Istapio.Application.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found")
    {
    }
}
