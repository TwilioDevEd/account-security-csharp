using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountSecurity {
    [Route("/")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/wwwroot/index.html");
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View("~/wwwroot/register/index.html");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("~/wwwroot/login/index.html");
        }

        [HttpGet("verification")]
        public IActionResult Verification()
        {
            return View("~/wwwroot/verification/index.html");
        }

        [HttpGet("2fa")]
        public IActionResult TwoFactorSample()
        {
            return View("~/wwwroot/2fa/index.html");
        }

        [HttpGet("protected")]
        [Authorize(Policy="AuthyVerified")]
        public IActionResult Protected()
        {
            return View("~/wwwroot/protected/index.html");
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}