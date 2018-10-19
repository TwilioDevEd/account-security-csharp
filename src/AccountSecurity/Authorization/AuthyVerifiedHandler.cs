using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AccountSecurity.Authorization
{
    internal class AuthyVerifiedHandler : AuthorizationHandler<AuthyVerifiedRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthyVerifiedRequirement requirement)
        {
            // if (context.User.HasClaim(c => c.Type == ClaimTypes.AuthyVerified))
            throw new System.NotImplementedException();
        }
    }
}