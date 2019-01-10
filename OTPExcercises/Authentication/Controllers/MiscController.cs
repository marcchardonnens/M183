using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Authentication.Controllers
{
    public class MiscController : Controller
    {

        public ActionResult OneTimePad()
        {
            return View();
        }

        public ActionResult CesarCipher()
        {
            return View();
        }

        public ActionResult VigenereCipher()
        {
            return View();
        }


    }
}