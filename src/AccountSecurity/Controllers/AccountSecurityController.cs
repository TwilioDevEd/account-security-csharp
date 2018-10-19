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

    [Produces("application/json")]
    [Route("/api/accountsecurity")]
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
                var result = await authy.verifyTokenAsync(currentUser.AuthyId, data.Token);
                logger.LogDebug(result.ToString());

                if (result.Succeeded)
                {
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
    }
}