using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace M183.Controllers
{
    public class HomeController : Controller
    {
        static string lastMessage = "";
        public ActionResult Index()
        {
            ViewBag.KeyloggerLastLine = lastMessage;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Aufgabensammlung()
        {
            return View();
        }



        [HttpGet]
        public ActionResult KeyLogger()
        {
            return View();
        }


        /// <summary>
        /// Aufgabe01
        /// </summary>
        /// <param name="line">letze abgeschickte line</param>
        /// <returns>Redirect to index</returns>
        [HttpGet]
        public ActionResult GetKeyLoggerLine(string line)
        {
            lastMessage = line;
            return RedirectToAction("Index");
        }
    }
}