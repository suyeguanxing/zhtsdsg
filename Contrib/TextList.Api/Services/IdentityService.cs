namespace RecAll.Contrib.TextList.Api.Services;

public class IdentityService : IIdentityService {
    private readonly IHttpContextAccessor _context;

    public IdentityService(IHttpContextAccessor context) {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public string GetUserIdentityGuid() =>
        _context.HttpContext.User.FindFirst("sub").Value;
}