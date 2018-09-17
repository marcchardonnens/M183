using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailOTP.Models
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string VerificationPW { get; set; }

    }
}