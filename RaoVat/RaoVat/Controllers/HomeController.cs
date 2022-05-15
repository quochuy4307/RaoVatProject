using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RaoVat.Models;

namespace RaoVat.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        DBRaoVatEntities db = new DBRaoVatEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View(db.RAOVATs.Include("User").OrderByDescending(x => x.NGAYGIODANG).Where(s => s.MATRANGTHAI == 1).Take(6).ToList());
        }
        public ActionResult TrangChu()
        {
            return View(db.RAOVATs.Include("User").OrderByDescending(x => x.NGAYGIODANG).Where(s => s.MATRANGTHAI == 1).Take(6).ToList());

        }
    }
}