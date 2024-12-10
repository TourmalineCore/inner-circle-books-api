using System.Security.Claims;

namespace Api
{
    public static class UserExtensions
    {
        private const string TenantIdClaimType = "tenantId";

        public static long GetTenantId(this ClaimsPrincipal context)
        {
            return long.Parse(context.FindFirstValue(TenantIdClaimType));
        }
    }
}
