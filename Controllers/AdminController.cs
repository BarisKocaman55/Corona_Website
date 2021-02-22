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
using Corono_Website.ViewModel.AdminPage;

namespace Corono_Website.Controllers
{
    public class AdminController : Controller
    {
        private DataBaseContext_Admin db = new DataBaseContext_Admin();


        [HttpGet]
        public ActionResult AdminRegister()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminRegister(Admins admin)
        {
            if (ModelState.IsValid)
            {
                var check = db.Admins.FirstOrDefault(x =>x.Email == admin.Email && x.Username == admin.Username);

                if(check == null)
                {
                    admin.Password = GetMD5(admin.Password);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Admins.Add(admin);
                    db.SaveChanges();
                    return RedirectToAction("CoronaTableEdit");
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
        public ActionResult AdminLogin()
        {
            return View(); 
        }

        [HttpPost]
        public ActionResult AdminLogin(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var pass = GetMD5(password);
                var data = db.Admins.Where(x => x.Username.Equals(username) && x.Password.Equals(pass)).ToList();

                if(data.Count() > 0)
                {
                    Session["Fullname"] = data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName;
                    Session["Email"] = data.FirstOrDefault().Email;
                    Session["AdminId"] = data.FirstOrDefault().AdminId;
                    return Redirect("/Admin/CoronaTableEdit/?user=" + username);
                }

                else
                {
                    ViewBag.error = "Login Failed";
                    return RedirectToAction("AdminLogin");
                }

            }
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("AdminLogin");
        }


        [HttpGet]
        public ActionResult CoronaTableEdit()
        {
            return View();
        }

        //Inserting Data into Corona Database
        [HttpPost]
        public ActionResult CoronaTableEdit(CoronaTable data)
        {
            DataBaseContext_Corona db_corona = new DataBaseContext_Corona();
            AdminPageViewModel model = new AdminPageViewModel();
                
            //Summarizing the Total Values
            var totalTest = Convert.ToInt32(db_corona.CoronaTable.Select(x => x.TotalTest).AsEnumerable().LastOrDefault());
            var totalCase = Convert.ToInt32(db_corona.CoronaTable.Select(x => x.TotalCase).AsEnumerable().LastOrDefault());
            var totalDeaths = Convert.ToInt32(db_corona.CoronaTable.Select(x => x.TotalDeaths).AsEnumerable().LastOrDefault());
            var totalHealed = Convert.ToInt32(db_corona.CoronaTable.Select(x => x.TotalHealed).AsEnumerable().LastOrDefault());

            totalTest = totalTest + data.NumOfTest;
            totalCase = totalCase + data.NumOfCase;
            totalDeaths = totalDeaths + data.NumOfDeaths;
            totalHealed = totalHealed + data.NumOfHealed;

            data.TotalTest = totalTest;
            data.TotalCase = totalCase;
            data.TotalDeaths = totalDeaths;
            data.TotalHealed = totalHealed;

            db_corona.CoronaTable.Add(data);

            int result = db_corona.SaveChanges();

            if(result > 0)
            {
                ViewBag.Result = "Veriler Başarıyla Eklenmiştir";
                ViewBag.Status = "success";
            }

            else
            {
                ViewBag.Result = "Verileri Ekleme İşlemi Gerçekleşememiştir!!!";
                ViewBag.Status = "danger";
            }
            

            return View();
        }

        [HttpGet]
        public ActionResult CoronaTableList()
        {
            DataBaseContext_Corona db_corona = new DataBaseContext_Corona();
            List<CoronaTable> corona_data = db_corona.CoronaTable.ToList();

            return View(corona_data);
        }

        //Deleting data from the DataBase
        [HttpGet]
        public ActionResult CoronaTableDelete(int? id)
        {
            CoronaTable data = null;

            if(id != null)
            {
                DataBaseContext_Corona db_corona = new DataBaseContext_Corona();
                data = db_corona.CoronaTable.Where(x => x.Id == id).FirstOrDefault();
            }

            return View(data);
        }

        [HttpPost, ActionName("CoronaTableDelete")]
        public ActionResult CoronaTableDelete_Post(int? id)
        {
            if(id != null)
            {
                DataBaseContext_Corona db_corona = new DataBaseContext_Corona();
                CoronaTable data = db_corona.CoronaTable.Where(x => x.Id == id).FirstOrDefault();

                db_corona.CoronaTable.Remove(data);
                int result = db_corona.SaveChanges();

                if (result > 0)
                {
                    ViewBag.Result = "Veriler Başarıyla Silinmiştir";
                    ViewBag.Status = "success";
                }

                else
                {
                    ViewBag.Result = "Verileri Silme İşlemi Gerçekleşememiştir!!!";
                    ViewBag.Status = "danger";
                }
            }
            return RedirectToAction("CoronaTableList", "Admin");
        }


        [HttpGet]
        public ActionResult CoronaTableUpdate(int? id)
        {
            CoronaTable data = null;
            if(id != null)
            {
                DataBaseContext_Corona db_corona = new DataBaseContext_Corona();
                data = db_corona.CoronaTable.Where(x => x.Id == id).FirstOrDefault();
            }
            return View(data);
        }

        [HttpPost]
        public ActionResult CoronaTableUpdate(CoronaTable model)
        {
            DataBaseContext_Corona db_corona = new DataBaseContext_Corona();
            CoronaTable data = db_corona.CoronaTable.Where(x => x.Id == model.Id).FirstOrDefault();

            if(data != null)
            {
                data.TotalTest = data.TotalTest - data.NumOfTest + model.NumOfTest;
                data.TotalCase = data.TotalCase - data.NumOfCase + model.NumOfCase;
                data.TotalHealed = data.TotalHealed - data.NumOfHealed + model.NumOfHealed;
                data.TotalDeaths = data.TotalDeaths - data.NumOfDeaths + model.NumOfDeaths;

                data.NumOfTest = model.NumOfTest;
                data.NumOfCase = model.NumOfCase;
                data.NumOfDeaths = model.NumOfDeaths;
                data.NumOfPatients = model.NumOfPatients;
                data.NumOfIntensiveCare = model.NumOfIntensiveCare;
                data.NumOfHealed = model.NumOfHealed;

                int result = db_corona.SaveChanges();

                if (result > 0)
                {
                    ViewBag.Result = "Veriler Başarıyla Güncellenmiştir";
                    ViewBag.Status = "success";
                }

                else
                {
                    ViewBag.Result = "Verileri Güncelleme İşlemi Gerçekleşememiştir!!!";
                    ViewBag.Status = "danger";
                }
            }

            return View();
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }

            return byte2String;
        }
    }
}