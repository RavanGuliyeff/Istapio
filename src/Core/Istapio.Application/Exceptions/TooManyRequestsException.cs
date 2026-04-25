namespace Istapio.Application.Exceptions;

public class TooManyRequestsException : BaseException
{
    public TooManyRequestsException(string message) : base(message)
    {
    }

    public TooManyRequestsException()
        : base("Too many requests. Please try again later")
    {
    }
}
