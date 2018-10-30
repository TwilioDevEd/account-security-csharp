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
    [Route("/api/user")]
    public class UserController : BaseController
    {
        public UserManager<ApplicationUser> userManager;
        public SignInManager<ApplicationUser> signInManager;
        public ILogger<UserController> logger;
        public IAuthy authy;

        public UserController(
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

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApplicationUser>> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { 
                    UserName = model.UserName,
                    Email = model.Email,
                    CountryCode = model.CountryCode,
                    PhoneNumber = model.PhoneNumber,
                    AuthyId = await authy.registerUserAsync(model)
                };


                var result = await this.userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await this.signInManager.SignInAsync(user, authenticationMethod: "AccountSecurityScheme", isPersistent: true);
                    logger.LogInformation(3, "User created a new account with password.");
                    return user;
                } else {
                    AddErrors(result);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApplicationUser>> Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await this.signInManager.PasswordSignInAsync(model.UserName, model.Password, true, false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(model.UserName);
                    var claims = await userManager.GetClaimsAsync(user);

                    logger.LogDebug("#########");
                    logger.LogDebug(JsonConvert.SerializeObject(claims));

                    await signInManager.SignOutAsync();
                    await this.signInManager.SignInAsync(user, authenticationMethod: "AccountSecurityScheme", isPersistent: true);

                    logger.LogDebug(JsonConvert.SerializeObject(user));

                    return user;
                } else {
                    return BadRequest(result);
                }

            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpGet("logout")]
        public async Task<ActionResult> logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "Home");
        }
    }
}