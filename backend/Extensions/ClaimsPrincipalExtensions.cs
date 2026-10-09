using System.Security.Claims;

namespace TaskMaster.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out var parsedUserId))
            throw new UnauthorizedAccessException("User ID not found in token");

        return parsedUserId;
    }
}
