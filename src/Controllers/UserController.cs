using System.Threading.Tasks;
using AccountSecurity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AccountSecurity {

    [Produces("application/json")]
    [Route("/api/user")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<UserController> logger;

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = loggerFactory.CreateLogger<UserController>();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApplicationUser>> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
                var result = await this.userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await this.signInManager.SignInAsync(user, isPersistent: false);
                    logger.LogInformation(3, "User created a new account with password.");
                    return user;
                }

                AddErrors(result);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApplicationUser>> Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName };
                await this.signInManager.SignInAsync(user, isPersistent: true);
                return user;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


    }
}