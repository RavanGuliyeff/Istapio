namespace Istapio.Application.Exceptions;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException()
        : base("Authentication is required")
    {
    }
}
