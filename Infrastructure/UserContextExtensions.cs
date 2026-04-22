using System.Security.Claims;

namespace Bake_Link.Infrastructure;

public static class UserContextExtensions
{
    public static int? GetRelatedId(this ClaimsPrincipal principal)
    {
        var raw = principal.FindFirstValue("related_id");
        return int.TryParse(raw, out var value) ? value : null;
    }

    public static string? GetRoleName(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Role) ?? principal.FindFirstValue("role");
    }
}
