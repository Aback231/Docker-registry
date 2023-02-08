using System.Globalization;

namespace webapi.Helpers.Exceptions;

public class IncorrectPasswordException : AppException
{
    public IncorrectPasswordException()
    {
    }

    public IncorrectPasswordException(string message) : base(message)
    {
    }

    public IncorrectPasswordException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}