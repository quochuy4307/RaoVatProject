using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RaoVat.Models;
using PagedList;
using PagedList.Mvc;
using System.Net.Mail;
using System.Net;


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
        public ActionResult Index(string category, int? page)
        {
            int pageSize = 4;
            int pageNum = (page ?? 1);
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
        public ActionResult Search(string keyword, string TENLOAI,string TENTHANHPHO, string GIA, int? page)
        {
            int pageSize = 4;
            int pageNum = (page ?? 1);
            if (GIA == "Giá Tăng Dần")
            {
                var lista = db.RAOVATs.OrderBy(x => x.GIA).Where(x=>x.MATRANGTHAI == 1).ToList();
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
        public ActionResult SuccesMail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendEmail(string receiver, string subject, string content, string subjectname, string subjectphone, string note)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var usercurrent = db.USERs.Where(x => x.EMAIL == receiver).FirstOrDefault();
                    var senderEmail = new MailAddress("quchuy0@gmail.com", "Thông Báo Rao Vặt");
                    var receiverEmail = new MailAddress(receiver, "Receiver");
                    var password = "180320Huy";
                    var sub = subject;
                    string body = content;
                    body = System.IO.File.ReadAllText(Server.MapPath("~/Views/Categories/email.html"));
                    body = body.Replace("{{username}}", usercurrent.TENDANGNHAP);
                    body = body.Replace("{{guessname}}", subjectname);
                    body = body.Replace("{{phone}}", subjectphone);
                    body = body.Replace("{{note}}", note);
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = subject,
                        IsBodyHtml = true,
                        Body = body,
                    })
                    {
                        smtp.Send(mess);
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.SendMail = "Some Error";
            }
            return RedirectToAction("SuccesMail", "Categories");
        }
    }
}