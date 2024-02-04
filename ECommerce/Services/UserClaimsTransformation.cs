using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ECommerce.Services
{
    public class UserClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity identity = new();

            if (!principal.HasClaim(c => c.Type == Constants.CurrentBusinessId)) 
            {
                identity.AddClaim(new Claim(Constants.CurrentBusinessId,string.Empty));
            }
            principal.AddIdentity(identity);
            return Task.FromResult(principal);
        }
    }

}
