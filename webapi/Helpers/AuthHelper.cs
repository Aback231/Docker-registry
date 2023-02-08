namespace webapi.Helpers;

public interface IAuthHelper
{
    Guid GetUserId(ControllerBase controller);
}

public class AuthHelper : IAuthHelper
{
    public Guid GetUserId(ControllerBase controller)
    {
        if (controller == null) throw new ArgumentNullException(nameof(controller));
        var identity = controller.HttpContext.User.Identity as ClaimsIdentity;
        if (identity == null) throw new UnauthorizedException();
        var nameClaim = identity.FindFirst(ClaimTypes.Name);
        if (nameClaim == null) throw new ApplicationException("Cannot get claim 'Name'.");
        return Guid.Parse(nameClaim.Value);
    }
}