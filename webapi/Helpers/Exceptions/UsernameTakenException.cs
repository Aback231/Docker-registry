using System.Globalization;

namespace webapi.Helpers.Exceptions;

public class UsernameTakenException : AppException
{
    public UsernameTakenException()
    {
    }

    public UsernameTakenException(string message) : base(message)
    {
    }

    public UsernameTakenException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}