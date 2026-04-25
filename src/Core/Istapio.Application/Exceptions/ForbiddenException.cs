namespace Istapio.Application.Exceptions;

public class ForbiddenException : BaseException
{
    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException()
        : base("You don't have permission to access this resource")
    {
    }
}
