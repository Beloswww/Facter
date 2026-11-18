using Newtonsoft.Json;
using prjFaceV.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace prjFaceV.Controllers
{
    [Authorize]
    public class TeacherController : Controller
    {
        DBMemberEntities3 db = new DBMemberEntities3();

        public string Course { get; private set; }
        public string Date { get; private set; }
        public string StudentID { get; private set; }
        public string Name { get; private set; }
        public string FileName { get; private set; }

        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Absence()
        {  
            string selectedCourse = (string)Session["selectedCourse"];
            string date = (string)Session["date"];
            ViewBag.test = Session["selectedCourse"];
            ViewBag.date = Session["date"];

            string similarImageName = Session["SimilarImageName"] as string;
            string similarImageName1 = Session["SimilarImageName1"] as string;
            string similarImageName2 = Session["SimilarImageName2"] as string;
            string similarImageName3 = Session["SimilarImageName3"] as string;
            Session["SimilarImageName3"] = null;
            Session["SimilarImageName"] = null;
            Session["SimilarImageNam1e"] = null;
            Session["SimilarImageName2"] = null;
            var rollcalls = db.rollcallTable.ToList();
            var totals = db.totalTable.ToList();

            foreach (var i in totals)
            {
                if ((i.FileName == similarImageName || i.FileName == similarImageName1 || i.FileName == similarImageName2 || i.FileName == similarImageName3) && i.Date == date && i.Course == selectedCourse)
                {
                    i.Absent += "出席";
                }
                else if (i.Date == date && i.Course == selectedCourse)
                {
                    i.Absent += "缺席";
                }
            }

            foreach (var i in rollcalls)
            {
                if ((i.FileName == similarImageName || i.FileName == similarImageName1 || i.FileName == similarImageName2 || i.FileName == similarImageName3))
                {
                    i.Absent += "出席";
                }
                else
                {
                    i.Absent += "缺席";
                }
            }

            db.SaveChanges();
            var rollcalltable = db.rollcallTable.ToList();
            return View(rollcalltable);
            //DirectoryInfo directory = new DirectoryInfo(Server.MapPath("~/MemberFaceImages"));
            //FileInfo[] fileInfos = directory.GetFiles();
            //return View(fileInfos);
        }
        public ActionResult Record()
        {
            var member = db.tMember.Where(m => m.TeacherID == User.Identity.Name).FirstOrDefault();
            ViewBag.Name = member.Name;
            ViewBag.TeacherID = member.TeacherID;
            ViewBag.Gmail = member.Gmail;
            return View();
        }
        [HttpPost]
        public ActionResult Record(string cDate, string selectedCourse)
        {
            var member = db.tMember.Where(m => m.TeacherID == User.Identity.Name).FirstOrDefault();
            ViewBag.Name = member.Name;
            ViewBag.TeacherID = member.TeacherID;
            ViewBag.Gmail = member.Gmail;

            var totaltable = db.totalTable
                .Where(m => m.Date == cDate && m.Course == selectedCourse).ToList();

            ViewBag.Date = cDate;
            ViewBag.Course = selectedCourse;


            if (totaltable.Count() <= 0)
            {
                ViewBag.Err = "no record";
                return View();
            }

            return View(totaltable);
        }


        public ActionResult TeacherChoice()
        {
            var member = db.tMember.Where(m => m.TeacherID == User.Identity.Name).FirstOrDefault();
            ViewBag.Name = member.Name;
            ViewBag.TeacherID = member.TeacherID;
            ViewBag.Gmail = member.Gmail;
            return View();
        }


        public ActionResult RollCall()
        {
            var rollcalltable = db.rollcallTable.ToList();
            foreach (var i in rollcalltable)
            {
                db.rollcallTable.Remove(i);
            }
            db.SaveChanges();

            var member = db.tMember.Where(m => m.TeacherID == User.Identity.Name).FirstOrDefault();
            ViewBag.Name = member.Name;
            ViewBag.TeacherID = member.TeacherID;
            ViewBag.Gmail = member.Gmail;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RollCall(FormCollection form, HttpPostedFileBase pic)
        {

            var member = db.tMember.Where(m => m.TeacherID == User.Identity.Name).FirstOrDefault();
            ViewBag.Name = member.Name;
            ViewBag.TeacherID = member.TeacherID;
            ViewBag.Gmail = member.Gmail;

            string date = form["date"];
            string selectedCourse = form["selectedCourse"];
            Session["selectedCourse"] = selectedCourse;
            Session["date"] = date;


            var tmpMember = db.sMember.ToList();
            // 創建一個新的 List<rollcallMember> 來保存 sMember 的內容
            List<totalTable> totalTable = new List<totalTable>();

            foreach (var i in tmpMember)
            {
                totalTable totaltables = new totalTable
                {
                    Course = selectedCourse,
                    Date = date,
                    Department = i.Department,
                    Grade = i.Grade,
                    StudentID = i.StudentID,
                    Name = i.Name,
                    FileName = i.FileName
                };

                totalTable.Add(totaltables);
            }

            // 將整個 List 一次性加入 rollcallMember 資料表
            db.totalTable.AddRange(totalTable);
            db.SaveChanges();

            List<rollcallTable> rollcallTable = new List<rollcallTable>();

            foreach (var i in tmpMember)
            {
                rollcallTable rollcalltable = new rollcallTable
                {
                    Department = i.Department,
                    Grade= i.Grade,
                    StudentID = i.StudentID,
                    Name = i.Name,
                    FileName = i.FileName
                };

                rollcallTable.Add(rollcalltable);
            }
            db.rollcallTable.AddRange(rollcallTable);
            db.SaveChanges();


            if (pic != null)
            {
                if (pic.ContentLength > 0)
                {

                    string filename = $"{Guid.NewGuid().ToString()}.jpg";
                    string savepath = Path.Combine(Server.MapPath("~/UploadFaceImages"), filename);
                    pic.SaveAs(savepath);

                    string filename1 = "4.jpg";
                    string savepath1 = Path.Combine(Server.MapPath("~/UploadFaceImages"), filename1);

                    string filename2 = "5.jpg";
                    string savepath2 = Path.Combine(Server.MapPath("~/UploadFaceImages"), filename2);

                    string filename3 = "6.jpg";
                    string savepath3 = Path.Combine(Server.MapPath("~/UploadFaceImages"), filename3);

                    string filename4 = "7.jpg";
                    string savepath4 = Path.Combine(Server.MapPath("~/UploadFaceImages"), filename4);

                    string apiUrl = "https://api.luxand.cloud/photo/similarity";
                    string apiKey = "YOUR_LUXAND_API_KEY";

                    using (HttpClient client = new HttpClient())
                    {

                        client.DefaultRequestHeaders.Add("token", apiKey);

                        string imagePath1, imagePath2;
                        imagePath1 = savepath1;

                        DirectoryInfo directory = new DirectoryInfo(Server.MapPath("~/MemberFaceImages"));
                        FileInfo[] fileInfos = directory.GetFiles();

                        foreach (var item in fileInfos)
                        {

                            byte[] fileBytes1 = System.IO.File.ReadAllBytes(imagePath1);  //上傳的圖
                            ByteArrayContent fileContent1 = new ByteArrayContent(fileBytes1);
                            fileContent1.Headers.Add("Content-Type", "application/octet-stream");

                            imagePath2 = item.FullName;
                            byte[] fileBytes2 = System.IO.File.ReadAllBytes(imagePath2);
                            ByteArrayContent fileContent2 = new ByteArrayContent(fileBytes2);
                            fileContent2.Headers.Add("Content-Type", "application/octet-stream");

                            //表單資料
                            var formData = new MultipartFormDataContent();
                            formData.Add(fileContent1, "face1", Path.GetFileName(imagePath1));
                            formData.Add(fileContent2, "face2", Path.GetFileName(imagePath2));
                            formData.Add(new StringContent("0.8"), "threshold"); // threshold值

                            // POST请求
                            HttpResponseMessage response = await client.PostAsync(apiUrl, formData);


                            // 檢查回應狀態
                            if (response.IsSuccessStatusCode)
                            {
                                string responseContent = await response.Content.ReadAsStringAsync();

                                臉部比對 face = JsonConvert.DeserializeObject<臉部比對>(responseContent);

                                if (face.similar == true && face.score > 0.90)
                                {
                                    Session["SimilarImageName"] = Path.GetFileName(imagePath2);
                                }
                            }
                            //else
                            //{
                            //    ViewBag.Err = $"API請求失敗，狀態碼:{response.StatusCode}";
                            //    return View();
                            //}
                        }
                    }


                    using (HttpClient client = new HttpClient())
                    {

                        client.DefaultRequestHeaders.Add("token", apiKey);

                        string imagePath1, imagePath2;
                        imagePath1 = savepath2;

                        DirectoryInfo directory = new DirectoryInfo(Server.MapPath("~/MemberFaceImages"));
                        FileInfo[] fileInfos = directory.GetFiles();

                        foreach (var item in fileInfos)
                        {

                            byte[] fileBytes1 = System.IO.File.ReadAllBytes(imagePath1);  //上傳的圖
                            ByteArrayContent fileContent1 = new ByteArrayContent(fileBytes1);
                            fileContent1.Headers.Add("Content-Type", "application/octet-stream");

                            imagePath2 = item.FullName;
                            byte[] fileBytes2 = System.IO.File.ReadAllBytes(imagePath2);
                            ByteArrayContent fileContent2 = new ByteArrayContent(fileBytes2);
                            fileContent2.Headers.Add("Content-Type", "application/octet-stream");

                            //表單資料
                            var formData = new MultipartFormDataContent();
                            formData.Add(fileContent1, "face1", Path.GetFileName(imagePath1));
                            formData.Add(fileContent2, "face2", Path.GetFileName(imagePath2));
                            formData.Add(new StringContent("0.8"), "threshold"); // threshold值

                            // POST请求
                            HttpResponseMessage response = await client.PostAsync(apiUrl, formData);


                            // 檢查回應狀態
                            if (response.IsSuccessStatusCode)
                            {
                                string responseContent = await response.Content.ReadAsStringAsync();

                                臉部比對 face = JsonConvert.DeserializeObject<臉部比對>(responseContent);

                                if (face.similar == true && face.score > 0.90)
                                {
                                    Session["SimilarImageName1"] = Path.GetFileName(imagePath2);
                                }
                            }
                            //else
                            //{
                            //    ViewBag.Err = $"API請求失敗，狀態碼:{response.StatusCode}";
                            //    return View();
                            //}
                        }
                    }

                    using (HttpClient client = new HttpClient())
                    {

                        client.DefaultRequestHeaders.Add("token", apiKey);

                        string imagePath1, imagePath2;
                        imagePath1 = savepath3;

                        DirectoryInfo directory = new DirectoryInfo(Server.MapPath("~/MemberFaceImages"));
                        FileInfo[] fileInfos = directory.GetFiles();

                        foreach (var item in fileInfos)
                        {

                            byte[] fileBytes1 = System.IO.File.ReadAllBytes(imagePath1);  //上傳的圖
                            ByteArrayContent fileContent1 = new ByteArrayContent(fileBytes1);
                            fileContent1.Headers.Add("Content-Type", "application/octet-stream");

                            imagePath2 = item.FullName;
                            byte[] fileBytes2 = System.IO.File.ReadAllBytes(imagePath2);
                            ByteArrayContent fileContent2 = new ByteArrayContent(fileBytes2);
                            fileContent2.Headers.Add("Content-Type", "application/octet-stream");

                            //表單資料
                            var formData = new MultipartFormDataContent();
                            formData.Add(fileContent1, "face1", Path.GetFileName(imagePath1));
                            formData.Add(fileContent2, "face2", Path.GetFileName(imagePath2));
                            formData.Add(new StringContent("0.8"), "threshold"); // threshold值

                            // POST请求
                            HttpResponseMessage response = await client.PostAsync(apiUrl, formData);


                            // 檢查回應狀態
                            if (response.IsSuccessStatusCode)
                            {
                                string responseContent = await response.Content.ReadAsStringAsync();

                                臉部比對 face = JsonConvert.DeserializeObject<臉部比對>(responseContent);

                                if (face.similar == true && face.score > 0.90)
                                {
                                    Session["SimilarImageName2"] = Path.GetFileName(imagePath2);
                                    //return RedirectToAction("Absence");
                                }
                            }
                            //else
                            //{
                            //    ViewBag.Err = $"API請求失敗，狀態碼:{response.StatusCode}";
                            //    return View();
                            //}
                        }
                    }

                    using (HttpClient client = new HttpClient())
                    {

                        client.DefaultRequestHeaders.Add("token", apiKey);

                        string imagePath1, imagePath2;
                        imagePath1 = savepath4;

                        DirectoryInfo directory = new DirectoryInfo(Server.MapPath("~/MemberFaceImages"));
                        FileInfo[] fileInfos = directory.GetFiles();

                        foreach (var item in fileInfos)
                        {

                            byte[] fileBytes1 = System.IO.File.ReadAllBytes(imagePath1);  //上傳的圖
                            ByteArrayContent fileContent1 = new ByteArrayContent(fileBytes1);
                            fileContent1.Headers.Add("Content-Type", "application/octet-stream");

                            imagePath2 = item.FullName;
                            byte[] fileBytes2 = System.IO.File.ReadAllBytes(imagePath2);
                            ByteArrayContent fileContent2 = new ByteArrayContent(fileBytes2);
                            fileContent2.Headers.Add("Content-Type", "application/octet-stream");

                            //表單資料
                            var formData = new MultipartFormDataContent();
                            formData.Add(fileContent1, "face1", Path.GetFileName(imagePath1));
                            formData.Add(fileContent2, "face2", Path.GetFileName(imagePath2));
                            formData.Add(new StringContent("0.8"), "threshold"); // threshold值

                            // POST请求
                            HttpResponseMessage response = await client.PostAsync(apiUrl, formData);


                            // 檢查回應狀態
                            if (response.IsSuccessStatusCode)
                            {
                                string responseContent = await response.Content.ReadAsStringAsync();

                                臉部比對 face = JsonConvert.DeserializeObject<臉部比對>(responseContent);

                                if (face.similar == true && face.score > 0.90)
                                {
                                    Session["SimilarImageName3"] = Path.GetFileName(imagePath2);
                                    return RedirectToAction("Absence");
                                }
                            }
                            else
                            {
                                ViewBag.Err = $"API請求失敗，狀態碼:{response.StatusCode}";
                                return View();
                            }
                        }
                    }

                }
            }
            ViewBag.Err = $"尚未註冊";
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}