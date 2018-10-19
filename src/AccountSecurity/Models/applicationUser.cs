using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AccountSecurity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string AuthyId { get; internal set; }
        public string CountryCode { get; internal set; }
    }
}
    

