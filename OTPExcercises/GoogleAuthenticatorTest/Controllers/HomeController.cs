using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Google.Authenticator;

namespace GoogleAuthenticatorTest.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

            var setupInfo = tfa.GenerateSetupCode("m183(marcchardonnens1@gmail.com)", "anothertestkey", 300, 300);

            ViewBag.Qr = "<img src=\"" + setupInfo.QrCodeSetupImageUrl + "\" />";
            ViewBag.Manual = setupInfo.ManualEntryKey;

            

            return View();
        }

        [HttpPost]
        public ActionResult Index(string token)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();

            bool isCorrectPin = tfa.ValidateTwoFactorPIN("randomkey", token);

            ViewBag.Qr = "<img src=\"" + setupInfo.QrCodeSetupImageUrl + "\" />";
            ViewBag.Manual = setupInfo.ManualEntryKey;


            return View();


        }
    }
}