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
        Route("/api/verification"),
        Produces("application/json")
    ]
    public class PhoneVerificationController : BaseController
    {
        public UserManager<ApplicationUser> userManager;
        public SignInManager<ApplicationUser> signInManager;
        public ILogger<PhoneVerificationController> logger;
        public IAuthy authy;

        public PhoneVerificationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILoggerFactory loggerFactory,
            IAuthy authy)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = loggerFactory.CreateLogger<PhoneVerificationController>();
            this.authy = authy;
        }

        [HttpPost("start")]
        public async Task<ActionResult> start([FromBody]PhoneVerificationRequestModel verificationRequest)
        {
            HttpContext.Session.Set<PhoneVerificationRequestModel>("phone_verification_request", verificationRequest);

            if (ModelState.IsValid)
            {
                string result;
                if (verificationRequest.via == Verification.SMS) {
                    result = await authy.phoneVerificationRequestAsync(
                        verificationRequest.CountryCode,
                        verificationRequest.PhoneNumber
                    );
                } else {
                    result = await authy.phoneVerificationCallRequestAsync(
                        verificationRequest.CountryCode,
                        verificationRequest.PhoneNumber
                    );
                }
               return Ok(result);
                
            } else {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("verify")]
        public async Task<ActionResult> verify([FromBody]TokenVerificationModel tokenVerification)
        {
            var verificationRequest = HttpContext.Session.Get<PhoneVerificationRequestModel>("phone_verification_request");

            if (ModelState.IsValid)
            {
                var validationResult = await authy.verifyPhoneTokenAsync(
                    verificationRequest.PhoneNumber,
                    verificationRequest.CountryCode,
                    tokenVerification.Token
                );

                return Ok(validationResult);
                
            } else {
                return BadRequest(ModelState);
            }
        }
    }
}