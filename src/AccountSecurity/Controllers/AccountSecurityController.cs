using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AccountSecurity.Models;
using AccountSecurity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AccountSecurity {

    [Authorize, Route("/api/accountsecurity"), Produces("application/json")]
    public class AccountSecurityController : BaseController
    {
        public UserManager<ApplicationUser> userManager;
        public SignInManager<ApplicationUser> signInManager;
        public ILogger<UserController> logger;
        public IAuthy authy;

        public AccountSecurityController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            IAuthy authy)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = loggerFactory.CreateLogger<UserController>();
            this.authy = authy;
        }

        [HttpPost("verify")]
        public async Task<ActionResult> verify([FromBody]TokenVerificationModel data)
        {
            var currentUser = await userManager.GetUserAsync(this.User);

            if (ModelState.IsValid)
            {
                TokenVerificationResult result;

                if(data.Token.Length > 4) {
                    result = await authy.verifyTokenAsync(currentUser.AuthyId, data.Token);
                } else {
                    result = await authy.verifyPhoneTokenAsync(currentUser.PhoneNumber, currentUser.CountryCode, data.Token);
                }

                logger.LogDebug(result.ToString());

                if (result.Succeeded)
                {
                    // Add TokenVerification claim to current user in order to allow them access
                    // to routes protected with AuthyTwoFactor authorization policy.
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.AuthenticationMethod, "TokenVerification")
                    };
                    await userManager.AddClaimsAsync(currentUser, claims);

                    return Ok(result);
                } else {
                    return BadRequest(result);
                }
                
            } else {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("sms")]
        public async Task<ActionResult> sms() 
        {
            var currentUser = await userManager.GetUserAsync(this.User);
            var result = await authy.sendSmsAsync(currentUser.AuthyId);
            return Ok(result);
        }

        [HttpPost("voice")]
        public async Task<ActionResult> voice()
        {
            var currentUser = await userManager.GetUserAsync(this.User);
            var result = await authy.phoneVerificationCallRequestAsync(currentUser);
            return Ok(result);
        }

    }
}