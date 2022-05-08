using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoDB2.Models;

namespace DemoDB2.Controllers
{
    public class AdminController : Controller
    {
        DBRaoVatEntities db = new DBRaoVatEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginAcc(FormCollection frc)
        {
            var _adname = frc["admin_name"];
            var _pass = frc["admin_pass"];
            var check = db.ADMINs.Where(s => s.TENDANGNHAP == _adname && s.MATKHAU == _pass).FirstOrDefault();
            if (check == null)
            {
                ViewBag.ErrorInfo = "Sai thông tin";
                return View("Index");
            }
            else
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                Session["TENDANGNHAP"] = _adname;
                Session["MATKHAU"] = _pass;
                return RedirectToAction("Index", "RaoVat");
            }
        }
        public ActionResult LogOutUser()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }
}