using System.ComponentModel.DataAnnotations;

namespace AccountSecurity.Models
{
    public class TokenVerificationModel
    {
        [Required]
        [StringLength(8, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        public string Token { get; set; }

    }
}