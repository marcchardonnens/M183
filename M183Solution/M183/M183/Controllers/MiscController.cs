using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace M183.Controllers
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

        public ActionResult XssVulnerableLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult XssVulnerableLogin(string username, string password)
        {
            SqlConnection db = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=..\\App_Data\\sql_xss_injection.mdf;Integrated Security=True;Connect Timeout=30");

            SqlCommand c = new SqlCommand();
            SqlDataReader reader;

            c.CommandText = "SELECT [Id], [Username], [Password] FROM [dbo].[User] WHERE [Username] = '" + username + "' AND [Password] = '" + password + "'";
            c.Connection = db;

            db.Open();

            reader = c.ExecuteReader();

            if(reader.HasRows)
            {
                ViewBag.Message = "success";
                while(reader.Read())
                {
                    ViewBag.Message += reader.GetInt32(0) + " " + reader.GetString(1) + " " + reader.GetString(2);
                }
            }
            else
            {
                ViewBag.Message = "Table is empty";
            }
            return View();
        }

        [HttpPost]
        public ActionResult GiveFeedback(string feedback)
        {
            SqlConnection db = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\M183\\M183Solution\\M183\\App_Data\\sql_xss_injection.mdf;Integrated Security=True;Connect Timeout=30");

            SqlCommand c = new SqlCommand();
            SqlDataReader reader;

            c.CommandText = "INSERT INTO [dbo].[Feedback] SET [Feedback] = '" + feedback + "'";
            c.Connection = db;

            db.Open();

            reader = c.ExecuteReader();

            if (reader.HasRows)
            {
                ViewBag.Message = "success";
                while (reader.Read())
                {
                    ViewBag.Message += reader.GetInt32(0) + " " + reader.GetString(1) + " " + reader.GetString(2);
                }
            }
            else
            {
                ViewBag.Message = "Table is empty";
            }
            return View();
        }

    }
}