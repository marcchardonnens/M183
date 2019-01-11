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
using System.Text;
using System.Net;
using System.IO;
using System.Net.Mail;

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
            client.Send("postmaster@m183.tk", message.Destination, message.Subject, message.Body);

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

    // Konfigurieren des in dieser Anwendung verwendeten Anwendungsbenutzer-Managers. UserManager wird in ASP.NET Identity definiert und von der Anwendung verwendet.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Konfigurieren der Überprüfungslogik für Benutzernamen.
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Konfigurieren der Überprüfungslogik für Kennwörter.
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Standardeinstellungen für Benutzersperren konfigurieren
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Registrieren von Anbietern für zweistufige Authentifizierung. Diese Anwendung verwendet telefonische und E-Mail-Nachrichten zum Empfangen eines Codes zum Überprüfen des Benutzers.
            // Sie können Ihren eigenen Anbieter erstellen und hier einfügen.
            manager.RegisterTwoFactorProvider("Telefoncode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Ihr Sicherheitscode lautet {0}"
            });
            manager.RegisterTwoFactorProvider("E-Mail-Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Sicherheitscode",
                BodyFormat = "Ihr Sicherheitscode lautet {0}"
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

    // Anwendungsanmelde-Manager konfigurieren, der in dieser Anwendung verwendet wird.
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
