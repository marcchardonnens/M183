using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

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
            VerificationNumber = random.Next(999999);
            

            //send email
            MailMessage mail = new MailMessage("debbie@isadork.com", Email);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("debbie.isadork@gmail.com", "debdeb123+");
            mail.Subject = "Login Verification";
            mail.Body = VerificationNumber.ToString();

            client.Send(mail);


        }
        public string Email { get;}
        public string Password { get;}

        public int VerificationNumber { get; set; }

    }
}