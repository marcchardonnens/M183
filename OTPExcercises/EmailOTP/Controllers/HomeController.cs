using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmailOTP;
using EmailOTP.ViewModels;
using EmailOTP.Models;

namespace EmailOTP.Controllers
{
    public class HomeController : Controller
    {
        Login ExampleLogin = new Login("marcchardonnens1@gmail.com","password");
        // GET: Home
        
        [HttpGet]
        public ActionResult Login()
        {


            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(LogInViewModel vm)
        {
            if(vm.Email == ExampleLogin.Email && vm.Password == ExampleLogin.Password)
            {
                
                //generate verification number and send email
                ExampleLogin.SendVerificationPW();

                 
                return View("Verification", new VerificationViewModel());
            }

            return View("Login",vm);
        }

        public ActionResult Index()
        {




            return View(new LogInViewModel());
        }

        [HttpPost]
        public ActionResult Verification(VerificationViewModel vm)
        {

            
            return View("verification");
        }


        [HttpPost]
        public ActionResult Verify(VerificationViewModel vm)
        {
            if(vm.VerificationCodeInput == ExampleLogin.VerificationNumber.ToString())
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Verification");
            
        }

    }
}