using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Cryptography;
using System;

namespace M183.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //Tutorial 5-TOTP
        public bool GoogleAuthVerified { get; set; }
        public string GoogleAuthSecret { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    //Tutorial 14-Logging und Audit Trails
    public class LoginLog
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime TimeCreated { get; set; }
        public bool Success { get; set; }

        //References in AccountController.cs
        public static void CreateLoginLog(string email, bool success, ApplicationUserManager manager)
        {
            ApplicationDbContext context = ApplicationDbContext.Create();
            LoginLog log = new LoginLog();
            log.TimeCreated = DateTime.Now;
            log.Success = success;
            log.Email = email;
            context.LoginLogs.Add(log);
            context.SaveChanges();
            if(!success)
            {
                if(context.LoginLogs.Where(x => x.Success == false && x.Email == email).ToList().Where(x => x.TimeCreated.AddHours(1) > DateTime.Now).Count() % 5 == 0)
                {
                    ApplicationUser user = context.Users.FirstOrDefault(x => x.Email == email);
                    if (user != null)
                    {
                        if (user.PhoneNumberConfirmed)
                        {

                            manager.SmsService.Send(new IdentityMessage()
                            {
                                Destination = user.PhoneNumber,
                                Body = "5 or more login attempts in the last hour."
                            });
                        }                   
                    }
                }
            }
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //Tutorial 14-Logging und Audit Trails
        public DbSet<LoginLog> LoginLogs { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}