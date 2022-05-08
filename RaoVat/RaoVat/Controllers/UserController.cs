using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using RaoVat.Models;

namespace RaoVat.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        DBRaoVatEntities db = new DBRaoVatEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LoginAcc(FormCollection frc)
        {
            var _username = frc["username"];
            var _pass = frc["pass"];
            var check = db.USERs.Where(s => s.TENDANGNHAP == _username && s.MATKHAU == _pass).FirstOrDefault();
            if (check == null)
            {
                ViewBag.ErrorInfo = "Sai thông tin";
                return View("Index");
            }
            else
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                Session["TENDANGNHAP"] = _username;
                Session["MATKHAU"] = _pass;
                return RedirectToAction("TrangChu", "Home");
            }
        }
        public ActionResult RegisterUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegisterUser(FormCollection frc)
        {
            var _usernname = frc["username"];
            var _name = frc["name"];
            var _phone = frc["phone"];
            var _email = frc["email"];
            var _pass = frc["pass"];
            var _repass = frc["repeat-pass"];
            if (ModelState.IsValid)
            {
                var check_id = db.USERs.Where(s => s.TENDANGNHAP == _usernname).FirstOrDefault();
                if (check_id == null)//chưa có id
                {
                    USER _user = new USER();
                    _user.TENDANGNHAP = _usernname;
                    _user.HOTEN = _name;
                    _user.SODIENTHOAI = _phone;
                    _user.EMAIL = _email;
                    _user.MATKHAU = _pass;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.USERs.Add(_user);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorRegister = "Tên đăng nhập đã tồn tại";
                    return View();
                }

            }
            return View();
        }
        public ActionResult LogOutUser()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
                db.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
                                                                // Confirm password does not match issue on save changes
                var v = db.USERs.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    db.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            ViewBag.Status = Status;
            return View();
        }
        [NonAction]
        public bool IsEmailExist(string emailID)
        {
                var v = db.USERs.Where(a => a.EMAIL == emailID).FirstOrDefault();
                return v != null;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] USER user)
        {
            bool Status = false;
            string message = "";
            //
            // Model Validation 
            if (ModelState.IsValid)
            {

                #region //Email is already Exist 
                var isExist = IsEmailExist(user.EMAIL);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                #endregion

                #region Generate Activation Code 
                user.ActivationCode = Guid.NewGuid();
                #endregion

                //#region  Password Hashing 
                //user.MATKHAU = Crypto.Hash(user.MATKHAU);
                //user.ConfirmPass = Crypto.Hash(user.ConfirmPass); //
                //#endregion
                user.IsEmailVerified = false;

                #region Save to Database
                    db.USERs.Add(user);
                    db.SaveChanges();

                    //Send Email to User
                    SendVerificationLinkEmail(user.EMAIL, user.ActivationCode.ToString());
                    message = "Registration successfully done. Account activation link " +
                        " has been sent to your email id:" + user.EMAIL;
                    Status = true;
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/User/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("raovat.huflit@gmail.com", "Rao Vặt");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "raovat2889"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Rao Vat account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Quên mật khẩu tài khoản RaoVat.com";
                body = "Chào bạn: " + emailID + "," +
                    "<br><br />Bạn vừa yêu cầu lấy lại mật khẩu tài khoản trên Website RaoVat.com." +
                    "<br><br /><b>Chú ý:</b> Bạn có thể bỏ qua email này nếu <b>người yêu cầu lấy lại mật khẩu không phải là bạn</b>" +
                    "<br><br />Hãy click vào dòng dưới đây để lấy lại mật khẩu của mình" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>" +
                    "<br><br />Nếu bạn muốn liên hệ với chúng tôi thì bạn đừng Reply lại theo email này!" +
                    "<br><br />Để biết thêm thông tin, xin hãy liên hệ với chúng tôi theo thông tin dưới đây:" +
                    "<br><br /><center><b>CÔNG TY TNHH RAO VẶT</b>" +
                    "<br><br />Địa chỉ: 363 Bình Trị Đông, Phường Bình Trị Đông A, Quận Bình Tân, TP Hồ Chí Minh" +
                    "<br><br />Website: https://raovat.vn </center>";

            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            //Verify Email ID
            //Generate Reset password link 
            //Send Email 
            string message = "";
            bool status = false;

                var account = db.USERs.Where(a => a.EMAIL == EmailID).FirstOrDefault();
                if (account != null)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.EMAIL, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                    //in our model class in part 1
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    message = "Yêu cầu cấp lại mật khẩu của quý khách đã được chấp nhận, một email đã được gửi tới " + EmailID  + " vui lòng kiểm tra email và làm theo hướng dẫn.";
                }
                else
                {
                    message = "Account not found";
                }
            ViewBag.Message = message;
            return View();
        }
        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

                var user = db.USERs.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                    var user = db.USERs.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.MATKHAU = /*Crypto.Hash(model.NewPassword);*/ model.NewPassword;
                        user.ResetPasswordCode = "";
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        message = "Mật khẩu tạo mới thành công";
                    }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }
}