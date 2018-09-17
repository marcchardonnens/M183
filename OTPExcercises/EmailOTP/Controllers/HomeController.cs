using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmailOTP.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewModels.LogInViewModel vm = new ViewModels.LogInViewModel();
            

            return View("index",vm);
        }


        [HttpPost]
        public ActionResult Verification(ViewModels.LogInViewModel viewmodel)
        {

            Models.LoginModel model = new Models.LoginModel();





            return View("verification");
        }
    }
}