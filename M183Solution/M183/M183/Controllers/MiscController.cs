﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using M183.Models;
using System.Text.RegularExpressions;

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

        //Tutorial 10-Database XSS - SQL Injection
        [HttpPost]
        public ActionResult XssVulnerableLogin(string username, string password)
        {
            //remove harmful characters
            username = Regex.Replace(username, @"[!@#$%_]", "");
            password = Regex.Replace(password, @"[!@#$%_]", "");

            //check path on your PC, relative path doesnt work
            SqlConnection db = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\M183\\M183Solution\\M183\\M183\\App_Data\\sql_xss_injection.mdf;Integrated Security=True;Connect Timeout=30");

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

        //Tutorial 10-Database XSS - SQL Injection
        [HttpPost]
        public ActionResult GiveFeedback(string feedback)
        {
            //remove harmful characters
            feedback = Regex.Replace(feedback, @"[!@#$%_]", "");

            //check path on your PC, relative path doesnt work
            SqlConnection db = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\M183\\M183Solution\\M183\\M183\\App_Data\\sql_xss_injection.mdf;Integrated Security=True;Connect Timeout=30");

            SqlCommand c = new SqlCommand();
            SqlDataReader reader;

            c.CommandText = "INSERT INTO [dbo].[Feedback] ([Feedback]) VALUES('" + feedback + "')";
            c.Connection = db;
           
            db.Open();

            reader = c.ExecuteReader();

            if (reader.HasRows)
            {
                ViewBag.Feedback = "success";
                while (reader.Read())
                {
                    ViewBag.Feedback += reader.GetInt32(0) + " " + reader.GetString(1) + " " + reader.GetString(2);
                }
            }
            else
            {
                ViewBag.Feedback = "Table is empty";
            }
            return RedirectToAction("XssVulnerableLogin");
        }

        //Tutorial 14-Logging und Audit Trails
        public ActionResult AllUserLogins(AllUserLoginViewModel model)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            List<AllUserLoginViewModel> models = new List<AllUserLoginViewModel>();

            foreach(LoginLog log in context.LoginLogs)
            {
                AllUserLoginViewModel vm = new AllUserLoginViewModel();
                vm.Email = log.Email;
                vm.Success = log.Success;
                vm.TimeCreated = log.TimeCreated;
                models.Add(vm);
            }
            return View(models);

        }

    }
}