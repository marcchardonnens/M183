using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailOTP.Models
{
    public class Login
    {
        public Login(string iEmail, string iPassword)
        {
            Email = iEmail;
            Password = iPassword;
        }

        public void SendVerificationPW()
        {
            Random random = new Random();
            verificationNumber = random.Next(999999);

            //send email
            
        }
        public string Email { get;}
        public string Password { get;}
        private int? verificationNumber = null;

        public int? VerificationNumber { get { return verificationNumber; } }

    }
}