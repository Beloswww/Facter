using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;

using prjFaceV.Models;

namespace prjFaceV.Controllers
{
    public class HomeController : Controller
    {
        DBMemberEntities3 db = new DBMemberEntities3();
        // GET: Home
        public ActionResult StudentRegister()
        {
            return View();
        }

        public ActionResult StudentRegisterSuccess()
        {
            return View();

        }

        public ActionResult TeacherRegisterSuccess()
        {
            return View();

        }

        public ActionResult StudentLogin()
        {
            return View();

        }



        [HttpPost]
        public ActionResult StudentLogin(string StudentID, string Password)
        {

            var member = db.sMember
                .Where(m => m.StudentID == StudentID && m.Password == Password)
                .FirstOrDefault();
            if (member == null)
            {
                ViewBag.Err = "Not Member";
                return View();
            }

            FormsAuthentication.RedirectFromLoginPage(member.StudentID, true);

            return RedirectToAction("StudentRecord", "Student");
        }

        [HttpPost]
        public ActionResult StudentRegister(sMember member, HttpPostedFileBase pic)
        {
            //對註冊的輸入資料做存儲
            var temp = db.sMember.Where(m => m.StudentID == member.StudentID).FirstOrDefault();
            if (temp != null)
            {
                ViewBag.Err = "Already Has This Person";
                return View();
            }

            if (pic != null)
            {
                if (pic.ContentLength > 0)
                {
                    string filename = $"{Guid.NewGuid().ToString()}.jpg";
                    string savepath = Path.Combine(Server.MapPath("~/MemberFaceImages"), filename);
                    pic.SaveAs(savepath);

                    member.FileName = filename;
                    db.sMember.Add(member);
                    db.SaveChanges();
                      
                }
            }
            // 註冊成功，將用戶重定向到 StudentRegisterSuccess 頁面
            return RedirectToAction("StudentRegisterSuccess");
        }



        public ActionResult TeacherLogin()
        {
            return View();

        }
        [HttpPost]
        public ActionResult TeacherLogin(string TeacherID, string Password)
        {

            var member = db.tMember
                .Where(m => m.TeacherID == TeacherID && m.Password == Password)
                .FirstOrDefault();
            if (member == null)
            {
                ViewBag.Err = "Not Member";
                return View();
            }

            FormsAuthentication.RedirectFromLoginPage(member.TeacherID, true);

            return RedirectToAction("TeacherChoice", "Teacher");
        }

        public ActionResult TeacherRegister()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TeacherRegister(tMember member)
        {
            //對註冊的輸入資料做存儲
            var temp = db.tMember.Where(m => m.TeacherID == member.TeacherID).FirstOrDefault();
            if (temp != null)
            {
                ViewBag.Err = "Has Register";
                return View();
            }
            db.tMember.Add(member);
            db.SaveChanges();

            return RedirectToAction("TeacherRegisterSuccess");
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}