using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Corono_Website.Models;
using Corono_Website.Models.Manager;
using System.Security.Cryptography;
using Corono_Website.Controllers;
using System.Text;

namespace Corono_Website.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult HomePage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RegisterPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterPage(User user)
        {
            DataBaseContext db = new DataBaseContext();
            if (ModelState.IsValid)
            {
                var check = db.Users.FirstOrDefault(x => x.Email == user.Email && x.Username == user.Username);
                if(check == null)
                {
                    user.Password = GetMD5(user.Password);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("CoronaTableList");
                }

                else
                {
                    ViewBag.error = "Email or Username already exists!!!";
                    return View();
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginPage(string username, string password)
        {
            DataBaseContext db = new DataBaseContext();
            if (ModelState.IsValid)
            {
                var pass = GetMD5(password);
                var data = db.Users.Where(x => x.Username.Equals(username) && x.Password.Equals(pass)).ToList();

                if(data.Count() > 0)
                {
                    Session["FullName"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["Userid"] = data.FirstOrDefault().Userid;
                    //return RedirectToAction("CoronaTableNews");
                    return Redirect("/Home/CoronaTableList/?user=" + username);
                }

                else
                {
                    ViewBag.error = "Login Failed";
                    return RedirectToAction("LoginPage");
                }

            }

            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear(); //Remove Session
            return RedirectToAction("LoginPage");
        }

        //List Corona Datas
        [HttpGet]
        public ActionResult CoronaTableList()
        {
            DataBaseContext_Corona db_corona = new DataBaseContext_Corona();
            List<CoronaTable> corona_data = db_corona.CoronaTable.ToList();

            ViewBag.Fullname = Session["Fullname"];

            return View(corona_data);
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for(int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }

            return byte2String;
        }


        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

    }
}