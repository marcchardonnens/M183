using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace Authentication.Controllers
{
    public class MiscController : Controller
    {

        SqlConnection s = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=..\\Sql_XssVulnerableDB\\sql_xss_injection.mdf;Integrated Security=True;Connect Timeout=30");

        public ActionResult XssVulnerableLogin()
        {
            return View();
        }

        public ActionResult Login(string Username, string Password)
        {


            return View();
        }

        public ActionResult GiveFeedback(string Feedback)
        {


            return View();
        }

    }
}