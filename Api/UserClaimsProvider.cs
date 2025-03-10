using System.Security.Claims;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;

namespace Api
{
    public class UserClaimsProvider : IUserClaimsProvider
    {
        public const string PermissionClaimType = "permissions";

        public const string CanViewBooks = "CanViewBooks";
        public const string CanManageBooks = "CanManageBooks";
        public const string IsBooksHardDeleteAllowed = "IsBooksHardDeleteAllowed";

        public Task<List<Claim>> GetUserClaimsAsync(string login)
        {
            throw new NotImplementedException();
        }
    }
}