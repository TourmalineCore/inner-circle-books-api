using System.Security.Claims;

namespace Api;

public static class UserExtensions
{
    private const string TenantIdClaimType = "tenantId";

    private const string CorporateEmailClaimType = "corporateEmail";

    public static long GetTenantId(this ClaimsPrincipal context)
    {
        return long.Parse(context.FindFirstValue(TenantIdClaimType));
    }

    public static string GetCorporateEmail(this ClaimsPrincipal context)
    {
        return context.FindFirstValue(CorporateEmailClaimType);
    }
}