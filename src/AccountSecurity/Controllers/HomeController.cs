using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountSecurity {
    [Route("/")]
    public class HomeController : Controller
    {
        [HttpGet]
        [HttpGet("index")]
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
        // [Authorize]
        public IActionResult Verification()
        {
            return View("~/wwwroot/verification/index.html");
        }

        [HttpGet("2fa")]
        // [Authorize]
        public IActionResult TwoFactorSample()
        {
            return View("~/wwwroot/2fa/index.html");
        }

        [HttpGet("protected")]
        // [Authorize]
        public IActionResult Protected()
        {
            return View("~/wwwroot/protected/index.html");
        }

        [HttpGet("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}