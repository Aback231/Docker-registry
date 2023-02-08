using System.Globalization;

namespace webapi.Helpers.Exceptions;

public class InvalidPasswordException : AppException
{
    public InvalidPasswordException()
    {
    }

    public InvalidPasswordException(string message) : base(message)
    {
    }

    public InvalidPasswordException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}