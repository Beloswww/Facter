using prjFaceV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace prjFaceV.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        // GET: Student
        DBMemberEntities3 db = new DBMemberEntities3();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StudentRecord()
        {
            var member = db.sMember.Where(m => m.StudentID == User.Identity.Name).FirstOrDefault();
            ViewBag.Department = member.Department;
            ViewBag.Grade = member.Grade;
            ViewBag.StudentID = member.StudentID;
            ViewBag.Name = member.Name;
            return View();
        }
        [HttpPost]
        public ActionResult StudentRecord(string selectedCourse)
        {
            ViewBag.SelectedCourse = selectedCourse;
            var member = db.sMember.Where(m => m.StudentID == User.Identity.Name).FirstOrDefault();
            ViewBag.Department = member.Department;
            ViewBag.Grade = member.Grade;
            ViewBag.StudentID = member.StudentID;
            ViewBag.Name = member.Name;

            var totaltable = db.totalTable
                .Where(m => m.Course == selectedCourse && m.StudentID == member.StudentID).ToList();
            var totaltable1 = db.totalTable
                .Where(m => m.Course == selectedCourse && m.Absent == "缺席" && m.StudentID == member.StudentID).ToList();


            if (totaltable1.Count() >= 3)
            {
                ViewBag.Err1 = "你要被當囉!  哈哈哈";
            }


            if (totaltable.Count() <= 0)
            {
                ViewBag.Err = "ao";
                return View();
            }

            return View(totaltable);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}