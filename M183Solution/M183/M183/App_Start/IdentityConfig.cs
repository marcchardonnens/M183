using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using M183.Models;
using System.Net.Mail;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Specialized;

namespace M183
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            //Aufgabe05
            var client = new SmtpClient("smtp.mailgun.org", 587)
            {
                Credentials = new System.Net.NetworkCredential("postmaster@m183.tk", "a0dd02505f71a90ea50aff0d7f60546a-060550c6-0d2b2677"),
                EnableSsl = true
            };
            //client.SendAsync("postmaster@m183.tk", message.Destination, message.Subject, message.Body, null);
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Hier den SMS-Dienst einfügen, um eine Textnachricht zu senden.

            //Aufgabe05 TODO

            //Nexmo.Api.SMS.SMSRequest request = new Nexmo.Api.SMS.SMSRequest
            //{
            //    from = "0798873151",
            //    to = message.Destination,
            //    text = message.Body
            //};

            //Nexmo.Api.Request.Credentials crd = new Nexmo.Api.Request.Credentials();
            //crd.ApiKey = "d02c1fb1";
            //crd.ApiSecret = "h1RcZMFUaK9hQIOC";

            //Nexmo.Api.SMS.Send(request,crd);

            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create("https://rest.nexmo.com/sms/json");

            string postData = "api_key=d02c1fb1";
            postData += "&api_secret=h1RcZMFUaK9hQIOC";
            postData += "&to=" + message.Destination;
            postData += "&from=\"M183\"";
            postData += "&text=\"" + message.Body + "\"";

            var data = Encoding.ASCII.GetBytes(postData);

            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.ContentLength = data.Length;

            using (var stream = httpRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            string response = new StreamReader(httpResponse.GetResponseStream()).ReadToEnd();

            //using (WebClient client = new WebClient())
            //{
            //    byte[] response = client.UploadValues("http://textbelt.com/text", new NameValueCollection() {
            //        { "phone", message.Destination },
            //        { "message", message.Body },
            //        { "key", "textbelt" }
            //    });

            //    string result = System.Text.Encoding.UTF8.GetString(response);
            //}

            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
