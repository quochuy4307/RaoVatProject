using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RaoVat.Models;
using PagedList;
using PagedList.Mvc;

namespace RaoVat.Controllers
{
    public class AdsController : Controller
    {
        // GET: Ads

        DBRaoVatEntities db = new DBRaoVatEntities();
        // GET: Ads
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult QuanLyTin(int? page)
        {
            int pageSize = 4;
            int pageNum = (page ?? 1);
            if (Session["TENDANGNHAP"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                var tendangnhap = Session["TENDANGNHAP"];
                var listTin = db.RAOVATs.Where(s => s.USER.TENDANGNHAP == tendangnhap).ToList();
                return View(listTin.ToPagedList(pageNum, pageSize));
            }
        }
        public PartialViewResult AdscatePartial()
        {
            return PartialView();
        }
        public ActionResult SelectState()
        {
            TINHTHANHPHO se_state = new TINHTHANHPHO();
            se_state.ListTinhThanhPho = db.TINHTHANHPHOes.ToList();
            return PartialView(se_state);
        }
        public ActionResult SelectCate()
        {
            CATEGORY se_cate = new CATEGORY();
            se_cate.ListCate = db.CATEGORies.ToList<CATEGORY>();
            return PartialView(se_cate);
        }
        public ActionResult SelectTinhTrang()
        {
            TINHTRANG se_tinhtrang = new TINHTRANG();
            se_tinhtrang.ListTinhTrang = db.TINHTRANGs.ToList<TINHTRANG>();
            return PartialView(se_tinhtrang);
        }
        public ActionResult SelectLoaiTin()
        {
            LOAITIN se_loaitin = new LOAITIN();
            se_loaitin.ListLoaiTin = db.LOAITINs.ToList<LOAITIN>();
            return PartialView(se_loaitin);
        }
        public ActionResult SelectHinhThuc()
        {
            HINHTHUC se_hinhthuc = new HINHTHUC();
            se_hinhthuc.ListHinhThuc = db.HINHTHUCs.ToList<HINHTHUC>();
            return PartialView(se_hinhthuc);
        }
        public ActionResult Create()
        {
            if (Session["TENDANGNHAP"] == null)
            {
                return RedirectToAction("Index", "User");
            }
            else 
            {
                RAOVAT raovat = new RAOVAT();
                return View(raovat); 
            }
        }
        [HttpPost]
        public ActionResult Create(RAOVAT raovat)
        {
            try
            {
                if(raovat.UploadImage !=null){
                    string filename = Path.GetFileNameWithoutExtension(raovat.UploadImage.FileName);
                    string extent = Path.GetExtension(raovat.UploadImage.FileName);
                    filename = filename + extent;
                    raovat.HINHANH1 = "~/Content/img/" + filename;
                    raovat.UploadImage.SaveAs(Path.Combine(Server.MapPath("~/Content/img/"), filename));
                }
                raovat.NGAYGIODANG = DateTime.Now;
                raovat.NGAYHETHAN = DateTime.Now.AddDays(30);
                raovat.MALOAITIN = 1;
                raovat.MATRANGTHAI = 2;
                var userTenDangNhap = Session["TENDANGNHAP"];
                var userCurrent = db.USERs.Where(x => x.TENDANGNHAP == userTenDangNhap.ToString()).FirstOrDefault();
                raovat.MANGUOIDUNG = Convert.ToInt32(userCurrent.MANGUOIDUNG);
                db.RAOVATs.Add(raovat);
                db.SaveChanges();
                return RedirectToAction("Index", "Categories");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Delete(int id)
        {
            return View(db.RAOVATs.Where(s => s.MATIN == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Delete(int id, RAOVAT raovat)
        {
            try
            {
                raovat = db.RAOVATs.Where(s => s.MATIN == id).FirstOrDefault();
                db.RAOVATs.Remove(raovat);
                db.SaveChanges();
                return RedirectToAction("QuanLyTin");
            }
            catch
            {
                return Content("This data is using in other table, ERROR DELETE");
            }
        }
        public ActionResult Edit(int id)
        {
            return View(db.RAOVATs.Where(s => s.MATIN == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Edit(int id, RAOVAT raovat)
        {
            raovat.NGAYGIODANG = DateTime.Now;
            raovat.MALOAITIN = 1;
            raovat.MATRANGTHAI = 2;
            var userTenDangNhap = Session["TENDANGNHAP"];
            var userCurrent = db.USERs.Where(x => x.TENDANGNHAP == userTenDangNhap.ToString()).FirstOrDefault();
            raovat.MANGUOIDUNG = Convert.ToInt32(userCurrent.MANGUOIDUNG);
            db.Entry(raovat).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("QuanLyTin");
        }
        public ActionResult DetailInfo(string id)
        {
            return View(db.USERs.Where(x=>x.TENDANGNHAP == id).FirstOrDefault());
        }
        public ActionResult EditUser(int id)
        {
            return View(db.USERs.Where(s => s.MANGUOIDUNG == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditUser(int id, USER user)
        {
            db.USERs.Attach(user);
            user.ErrorLogin = "NULL";
            if (user.GIOITINH == null)
            {
                user.GIOITINH = false;
            }

            if (user.NGAYSINH == null)
            {
                user.NGAYSINH = DateTime.Now;
            }
            if (user.DIACHI == null)
            {
                user.DIACHI = "NULL";
            }
            if (user.ResetPasswordCode == null)
            {
                user.ResetPasswordCode = Guid.NewGuid().ToString();
            }
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("QuanLyTin");
        }
    }
}