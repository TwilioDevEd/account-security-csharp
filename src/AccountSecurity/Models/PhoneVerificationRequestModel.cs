using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AccountSecurity.Models
{
    public class PhoneVerificationRequestModel
    {
        public PhoneVerificationRequestModel() {
            this.via = Verification.SMS;
        }

        [Required]
        [StringLength(4, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string CountryCode { get; set; }

        [Required]
        [StringLength(16, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 7)]
        public string PhoneNumber { get; set; }

        public Verification via { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Verification
    {
        SMS,
        CALL
    }
}