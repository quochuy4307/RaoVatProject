using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RaoVat.Models;
using PagedList;
using PagedList.Mvc;

namespace RaoVat.Controllers
{
    public class CategoriesController : Controller
    {
        // GET: Categories
        DBRaoVatEntities db = new DBRaoVatEntities();
        public ActionResult Detail(int? id)
        {
           if(id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            return View(db.RAOVATs.Where(x => x.MATIN == id).FirstOrDefault());
        }
        public ActionResult Index(string category, int? page, string sortBy)
        {
            int pageSize = 4;
            int pageNum = (page ?? 1);
            ViewBag.PriceHighLow = "Price_desc";
            ViewBag.PriceLowHigh = "Price_asc";
            ViewBag.Newess = "New";
            if (sortBy != null)
            {
                switch (sortBy)
                {
                    case "Price_desc":
                        var listsort = db.RAOVATs.OrderByDescending(x => x.GIA).ToList();
                        return View(listsort.ToPagedList(pageNum, pageSize));
                    case "Price_asc":
                        var listsort1 = db.RAOVATs.OrderBy(x => x.GIA).ToList();
                        return View(listsort1.ToPagedList(pageNum, pageSize));
                    case "New":
                        var listsort2 = db.RAOVATs.OrderByDescending(p => p.NGAYGIODANG).ToList();
                        return View(listsort2.ToPagedList(pageNum, pageSize));
                    default:
                        break;
                }
            }
            if (category == null)
            {
                var list = db.RAOVATs.OrderByDescending(x => x.NGAYGIODANG).Where(s => s.MATRANGTHAI == 1);
                return View(list.ToPagedList(pageNum,pageSize));
            }
            else
            {
                var list = db.RAOVATs.OrderByDescending(x => x.NGAYGIODANG).Where(x => x.CATEGORY.TENLOAI == category && x.MATRANGTHAI == 1);
                return View(list.ToPagedList(pageNum, pageSize));
            }
        }
        [HttpGet]
        public ActionResult Search(string keyword, string TENLOAI, string TENTHANHPHO, string GIA, int? page)
        {
            int pageSize = 4;
            int pageNum = (page ?? 1);
            if (GIA == "Giá Tăng Dần")
            {
                var lista = db.RAOVATs.OrderBy(x => x.GIA).Where(x => x.MATRANGTHAI == 1).ToList();
                return View(lista.ToPagedList(pageNum, pageSize));
            }
            if (GIA == "Giá Giảm Dần")
            {
                var lista = db.RAOVATs.OrderByDescending(x => x.GIA).Where(x => x.MATRANGTHAI == 1).ToList();
                return View(lista.ToPagedList(pageNum, pageSize));
            }
            if (TENLOAI == "Xe" || TENLOAI == "Đồ điện tử" || TENLOAI == "Hôn nhân" || TENLOAI == "Nội thất" || TENLOAI == "Công việc" || TENLOAI == "Địa ốc" || TENLOAI == "Sức khỏe và làm đẹp" || TENLOAI == "Thời trang" || TENLOAI == "Giáo dục" || TENLOAI == "Dụng cụ" || TENLOAI == "Du lịch")
            {
                var lista = db.RAOVATs.Include("CATEGORY").OrderByDescending(x => x.NGAYGIODANG).Where(p => p.CATEGORY.TENLOAI == TENLOAI).Where(s => s.MATRANGTHAI == 1).ToList();
                return View(lista.ToPagedList(pageNum, pageSize));
            }
            if (TENTHANHPHO == "Thành phố Hà Nội" || TENTHANHPHO == "Thành phố Hải Phòng" || TENTHANHPHO == "Thành phố Đà Nẵng" || TENTHANHPHO == "Thành phố Hồ Chí Minh" || TENTHANHPHO == "Thành phố Cần Thơ")
            {
                var listb = db.RAOVATs.Include("TINHTHANHPHO").OrderByDescending(x => x.NGAYGIODANG).Where(p => p.TINHTHANHPHO.TENTHANHPHO == TENTHANHPHO).Where(s => s.MATRANGTHAI == 1).ToList();
                return View(listb.ToPagedList(pageNum, pageSize));
            }
            if (keyword != null)
            {
                var search = db.RAOVATs.OrderByDescending(x => x.NGAYGIODANG).Where(x => x.TIEUDE.Contains(keyword) && x.MATRANGTHAI == 1).ToList();
                return View(search.ToPagedList(pageNum, pageSize));
            }
            var list = db.RAOVATs.OrderByDescending(x => x.NGAYGIODANG).Where(s => s.MATRANGTHAI == 1);
            return View(list.ToPagedList(pageNum, pageSize));
        }
        public PartialViewResult CategoryPartial()
        {
            var catelist = db.CATEGORies.ToList();
            return PartialView(catelist);
        }
        public ActionResult SearchOption(double min = double.MinValue, double max = double.MaxValue)
        {
            var listsear = db.RAOVATs.OrderByDescending(x => x.NGAYGIODANG).Where(p => (double)p.GIA >= min && (double)p.GIA <= max).Where(s => s.MATRANGTHAI == 1).ToList();
            return View(listsear);
        }
    }
}