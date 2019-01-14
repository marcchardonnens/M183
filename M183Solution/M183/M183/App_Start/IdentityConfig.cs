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
using Google.Authenticator;

namespace M183
{
    public class EmailService : IIdentityMessageService
    {
        //Tutorial 5-OTP
        public Task SendAsync(IdentityMessage message)
        {
            var client = new SmtpClient("smtp.mailgun.org", 587)
            {
                //these credentials were working for a week
                //the domain is still marked as active at mailguns dashboard
                //however after some testing over the weekend it suddenly stopped working
                //the sandboxone doesnt work either
                //i suspect my mailgun account may have gotten flagged :(
                //the code should work just fine with a different account
                Credentials = new NetworkCredential("postmaster@m183.tk", "Wc3BestG4me!"),
                EnableSsl = true
            };
            client.SendAsync("postmaster@m183.tk", message.Destination, message.Subject, message.Body, null);
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        //Tutorial 5-OTP
        public Task SendAsync(IdentityMessage message)
        {

            //this only works with testnumbers that have been registered at nexmo.
            //to use the full feature i would have had to pay $10
            //use your own api key and secret and register your number and all sms communication will work
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
            return Task.FromResult(0);
        }
    }

    //EmailTokenProvider<TUser> : EmailTokenProvider<TUser, string> where TUser : class, IUser<string>
    public class GoogleTokenProvider<TUser> : IUserTokenProvider<ApplicationUser, string>
    {
        public Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser, string> manager, ApplicationUser user)
        {
            //current code of user
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            return Task.FromResult(tfa.GetCurrentPIN(user.GoogleAuthSecret));
        }

        public Task<bool> IsValidProviderForUserAsync(UserManager<ApplicationUser, string> manager, ApplicationUser user)
        {
            //check if GoogleAuth was enabled
            return Task.FromResult(user.GoogleAuthVerified);
        }

        public Task NotifyAsync(string token, UserManager<ApplicationUser, string> manager, ApplicationUser user)
        {
            //yes.
            return Task.FromResult(0);
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser, string> manager, ApplicationUser user)
        {
            //validate userinput with current token with corresponding user secret
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            return Task.FromResult(tfa.ValidateTwoFactorPIN(user.GoogleAuthSecret, tfa.GetCurrentPIN(user.GoogleAuthSecret)));
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
            //Tutorial 5 - OTP
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            //Tutorial 5-TOTP
            //add as two factor option
            manager.RegisterTwoFactorProvider("Google Authenticator", new GoogleTokenProvider<ApplicationUser>());

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
