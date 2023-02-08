using System.Globalization;

namespace webapi.Helpers.Exceptions;

public class TooManyFailedLoginAttemptsException : AppException
{
    public TooManyFailedLoginAttemptsException()
    {
    }

    public TooManyFailedLoginAttemptsException(string message) : base(message)
    {
    }

    public TooManyFailedLoginAttemptsException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}