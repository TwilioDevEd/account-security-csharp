using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AccountSecurity.Models;
using AccountSecurity.Services;
using AccountSecurity.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AccountSecurity {

    [
        Authorize,
        Route("/api/accountsecurity"),
        Produces("application/json")
    ]
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

        internal async Task<IList<Claim>> addTokenVerificationClaim(ApplicationUser user) {
            var tokenVerificationClaim = new Claim(ClaimTypes.AuthenticationMethod, "TokenVerification");
            var claims = new List<Claim>();
            claims.Add(tokenVerificationClaim);

            var userClaims = (List<Claim>)await userManager.GetClaimsAsync(user);

            if (userClaims.FindIndex(claim => claim.Value.Equals("TokenVerification")) == -1) {
                await userManager.AddClaimsAsync(user, claims);
            }

            return await userManager.GetClaimsAsync(user);
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
                    await addTokenVerificationClaim(currentUser);
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
            var result = await authy.phoneVerificationCallRequestAsync(
                currentUser.CountryCode,
                currentUser.PhoneNumber
            );
            return Ok(result);
        }

        [HttpPost("onetouch")]
        public async Task<ActionResult> oneTouch()
        {
            var currentUser = await userManager.GetUserAsync(this.User);
            var onetouch_uuid = await authy.createApprovalRequestAsync(currentUser.AuthyId);
            HttpContext.Session.Set<string>("onetouch_uuid", onetouch_uuid);
            return Ok();
        }

        [HttpPost("onetouchstatus")]
        public async Task<ActionResult> oneTouchStatus()
        {
            var currentUser = await userManager.GetUserAsync(this.User);
            var onetouch_uuid = HttpContext.Session.Get<string>("onetouch_uuid");

            var result = await authy.checkRequestStatusAsync(onetouch_uuid);
            var data = (JObject)result;
            var approval_request_status = (string)data["approval_request"]["status"];

            if (approval_request_status == "approved") {
                await addTokenVerificationClaim(currentUser);
                await userManager.UpdateAsync(currentUser);
            }


            return Ok(result);
        }
    }
}