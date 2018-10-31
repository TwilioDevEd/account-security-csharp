using System.Threading.Tasks;
using AccountSecurity.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AccountSecurity {
    [Route("/")]
    public class HomeController : Controller
    {
        public SignInManager<ApplicationUser> signInManager;

        public HomeController(SignInManager<ApplicationUser> signInManager) {
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous, HttpGet("index")]
        public IActionResult Index()
        {
            return View("~/wwwroot/index.html");
        }

        [AllowAnonymous, HttpGet("register")]
        public IActionResult Register()
        {
            return View("~/wwwroot/register/index.html");
        }

        [AllowAnonymous, HttpGet("login")]
        public IActionResult Login()
        {
            return View("~/wwwroot/login/index.html");
        }

        [Authorize, HttpGet("logout")] 
        public async Task<IActionResult> Logout() { 
            await signInManager.SignOutAsync(); 
            return RedirectToAction("Index", "Home"); 
        } 

        [HttpGet("verification")]
        public IActionResult Verification()
        {
            return View("~/wwwroot/verification/index.html");
        }

        [HttpGet("verified")]
        public IActionResult Verified()
        {
            return View("~/wwwroot/verified/index.html");
        }

        [Authorize, HttpGet("2fa")]
        public IActionResult TwoFactorSample()
        {
            return View("~/wwwroot/2fa/index.html");
        }

        [Authorize, HttpGet("protected")]
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