using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EventWise.Api;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid UserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out var result) 
            ? result 
            : throw new ApplicationException("User ID not found in claims.");
    }
}
