using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//step 1
using ProjectSem03.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Dynamic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace ProjectSem03.Controllers
{
    public class StudentDesignPostingController : Controller
    {
        //step 2
        ProjectDB db;
        public StudentDesignPostingController(ProjectDB db)
        {
            this.db = db;
        }

        //step 3
        //INDEX for Student View Design
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("studentid") == null) //check session
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                var list = from d in db.Design
                           join s in db.Student on d.StudentId equals s.StudentId
                           where s.StudentId.Equals(HttpContext.Session.GetString("studentid")) //check student
                           select new CombineModels
                           {
                               Designs = d,
                               Students = s
                           };
               return View(list);
            }
        }

        //EDIT
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("studentid") == null) //check session
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                var model = db.Design.Find(id);
                //check model is not null and this DesignStudentId == Session
                if (model != null && model.StudentId.Equals(HttpContext.Session.GetString("studentid")))
                {
                    return View(model);
                }
                else
                {
                    return NotFound();
                }
            } //end check session
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 8388608)]
        [RequestSizeLimit(8388608)]
        public async Task<IActionResult> Edit(Design design, IFormFile file, [FromServices] IWebHostEnvironment owebHostEnvironment)
        {
            try
            {
                var model = db.Design.SingleOrDefault(s => s.DesignId.Equals(design.DesignId));
                if (ModelState.IsValid)
                {
                    if (model != null)
                    {
                        //get DesignId for check Competition
                        var modelPosting = db.Posting.SingleOrDefault(p => p.DesignID.Equals(model.DesignId));                        
                        var modelCompetition = db.Competition.SingleOrDefault(c => c.CompetitionId.Equals(modelPosting.CompetitionId));

                        //get staff manage this competition
                        var modelStudent = db.Student.SingleOrDefault(st=>st.StudentId.Equals(model.StudentId));
                        var modelStaff = db.Staff.SingleOrDefault(s => s.StaffId.Equals(modelCompetition.StaffId));
                        string newFilePath = ""; //images after update

                        var today = DateTime.Now;                        
                        ////check today SubmitDate                     
                        if (today >= modelCompetition.StartDate.Date && today <= modelCompetition.EndDate.Date)
                        {
                            if(modelPosting.Mark!=null || modelPosting.Remark != null)
                            {
                                ViewBag.Msg = "This painting has been graded";
                            }
                            else if (file == null) //if no change painting
                            {
                                model.DesignName = design.DesignName;
                                model.Description = design.Description;
                                model.Price = design.Price;
                                //add posting
                                modelPosting.PostDate = today;
                                db.SaveChanges();
                                //message box
                                string message = "File updated Successful";
                                TempData["message"] = "<script>alert('" + message + "');</script>";
                                newFilePath = Path.Combine("wwwroot/images/Medium", model.Painting);
                                await SendMailGoogleSmtp("phathuyhuukhanh@gmail.com", modelStaff.Email, "[" + modelStudent.StudentId + "]-[" + modelStudent.FirstName + " " + modelStudent.LastName + "] Has updated the drawing design", "<p><strong>PostingId:" + modelPosting.PostingId + "</strong></p><h3>DesignName: " + design.DesignName + "</h3><p>Description: " + design.Description + "</p><p>StudentName: " + modelStudent.FirstName + " " + modelStudent.LastName + "</p><br/>" + "", "phathuyhuukhanh@gmail.com", "t+NShmKmHyq0H7kp7ZBVRg==", newFilePath); //smtp gmail: phathuyhuukhanh@gmail.com

                                return RedirectToAction("Upload", "Home");
                            }
                            else
                            {

                                string ext = Path.GetExtension(file.FileName);
                                if ((file.Length > 0 && file.Length < 8388608) && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".png"))) //painting must be .jpg or .png
                                {
                                    string tempCurFilePath = Path.Combine("wwwroot/images/Medium", model.Painting); //old painting
                                    string renameFile = Convert.ToString(Guid.NewGuid()) + today.ToString("_yyyy-MM-dd_hhmmss_tt") + ext;
                                    string fileNameAndPath = $"{owebHostEnvironment.WebRootPath}\\images\\Medium\\{renameFile}";

                                    using (var stream = new FileStream(fileNameAndPath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                        await stream.FlushAsync();
                                    }                                    
                                    model.DesignName = design.DesignName;
                                    design.Painting = renameFile;
                                    model.Painting = design.Painting;
                                    model.Description = design.Description;
                                    model.Price = design.Price;
                                    //Student cannot change DesignId and StudentId and SubmitDate >= StartDate && <= EndDate of Competition 
                                    db.SaveChanges();
                                    //update posting
                                    modelPosting.PostDate = today;
                                    db.SaveChanges();
                                                                       
                                    //messagebox
                                    string message = "File updated Successful";
                                    TempData["message"] = "<script>alert('" + message + "');</script>";
                                    newFilePath = Path.Combine("wwwroot/images/Medium", model.Painting); //new painting
                                    await SendMailGoogleSmtp("phathuyhuukhanh@gmail.com", modelStaff.Email, "[" + modelStudent.StudentId + "]-[" + modelStudent.FirstName + " " + modelStudent.LastName + "] Has updated the drawing design", "<p><strong>PostingId:" + modelPosting.PostingId + "</strong></p><h3>DesignName: " + design.DesignName + "</h3><p>Description: " + design.Description + "</p><p>StudentName: " + modelStudent.FirstName + " " + modelStudent.LastName + "</p><br/>" + "", "phathuyhuukhanh@gmail.com", "t+NShmKmHyq0H7kp7ZBVRg==", newFilePath); //smtp gmail: phathuyhuukhanh@gmail.com
                                    System.GC.Collect();
                                    System.GC.WaitForPendingFinalizers();
                                    //check old painting exists
                                    if (System.IO.File.Exists(tempCurFilePath))
                                    {
                                        System.IO.File.Delete(tempCurFilePath);
                                    }

                                    return RedirectToAction("Upload", "Home");
                                }
                                else if (file.Length > 8388608)
                                {
                                    ViewBag.Msg = "Painting must be smaller than 8MB";
                                }
                                else
                                {
                                    ViewBag.Msg = "Painting must be .jpg or .png";
                                }
                            }
                        }
                        else
                        {
                            ViewBag.Msg = "Time to submit pictures for competition has expired ( Endate: " + modelCompetition.EndDate.ToString() + " )";                            
                        }// End check Design Submitdate
                    }//END check model
                    else
                    {
                        ViewBag.Msg = "Failed";
                    }
                }
                else
                {
                    ViewBag.Msg = "Model Failed";
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message;
            }
            return View();
        }

        //Student Register to Join Competition
        //1. View InCommingCompetition
        public IActionResult InCommingCompetition()
        {
            DateTime today = Convert.ToDateTime(DateTime.Today);
            var list = (from c in db.Competition
                       where today >= c.StartDate.Date && today <= c.EndDate.Date
                       select c).ToList();
            return View(list);
        }

        //2. UPDATE to join
        private List<CombineModels> designStudentList()
        {
            var list = (from d in db.Design
                        join s in db.Student on d.StudentId equals s.StudentId
                        where s.StudentId.Equals(HttpContext.Session.GetString("studentid")) //check student
                        select new CombineModels
                        {
                            Designs = d,
                            Students = s
                        }).ToList();
            return list;
        }
        private bool CheckPosting(int id, string stuId)
        {
            //check if student is already registered competition
            var postList = (from p in db.Posting
                            join d in db.Design on p.DesignID equals d.DesignId
                            where p.CompetitionId == id && d.StudentId.Equals(stuId) && p.DesignID.Equals(d.DesignId)
                            select new CombineModels
                            {
                                Designs = d,
                                Postings = p
                            }).ToList();

            if (postList.Count > 0) //check row
            {
                return true;
            }
            else
            {                
                return false;
            }
        }
        //UPLOAD   
        public IActionResult Upload(int id)
        {
            string stuId = HttpContext.Session.GetString("studentid");
            if (stuId == null) //check login
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                //check root                
                var compList = db.Competition.SingleOrDefault(c=>c.CompetitionId.Equals(id));
                if (compList == null)
                {
                    return NotFound();
                }

                //Viewbag list of student designs
                //ViewBag.designList = designStudentList();

                //check if student is already registered competition
                if(CheckPosting(id, stuId) == true)
                {
                    TempData["testmsg"] = "<script>alert('You have already registered for this competition');</script>";
                    //ViewBag.Msg = "You have already registered for this competition";
                    //return View();
                    return RedirectToAction("Index", "Home");
                }                                
                else
                {
                    HttpContext.Session.SetInt32("registerCompetitionId", id); // competitionId
                    return View();
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 8388608)]
        [RequestSizeLimit(8388608)]
        public async Task<IActionResult> Upload(Design design, IFormFile file, [FromServices] IWebHostEnvironment owebHostEnvironment)
        {
            //ViewBag list of student designs
            //ViewBag.designList = designStudentList();
            try
            {
                if (ModelState.IsValid)
                {
                    var student = db.Student.SingleOrDefault(s => s.StudentId.Equals(HttpContext.Session.GetString("studentid")));
                    int competitionId = (int)HttpContext.Session.GetInt32("registerCompetitionId");
                    var comp = db.Competition.SingleOrDefault(c => c.CompetitionId.Equals((int)HttpContext.Session.GetInt32("registerCompetitionId")));

                    //check if student is already registered competition
                    if (CheckPosting(competitionId, student.StudentId) == true)
                    {
                        TempData["testmsg"] = "<script>alert('You have already registered for this competition');</script>";
                        return RedirectToAction("Index", "Home");
                    }
                    //get staff manage this competition
                    var modelStaff = db.Staff.SingleOrDefault(s => s.StaffId.Equals(comp.StaffId));
                    string newFilePath = "";

                    var today = DateTime.Now;

                    ////check today SubmitDate                     
                    if (today >= comp.StartDate.Date && today <= comp.EndDate.Date) //null exception
                    {
                        if (file == null)
                        {
                            ViewBag.Msg = "Painting is required";
                        }
                        else {
                            string ext = Path.GetExtension(file.FileName);
                            if (file != null && (file.Length > 0 && file.Length < 8388608) && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".png"))) //painting must be .jpg or .png
                            {
                                string renameFile = Convert.ToString(Guid.NewGuid()) + today.ToString("_yyyy-MM-dd_hhmmss_tt") + ext;
                                string fileNameAndPath = $"{owebHostEnvironment.WebRootPath}\\images\\Medium\\{renameFile}";
                                using (var stream = new FileStream(fileNameAndPath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                    await stream.FlushAsync();
                                }

                                design.Painting = renameFile;
                                design.StudentId = HttpContext.Session.GetString("studentid"); //login session
                                db.Design.Add(design);
                                db.SaveChanges();
                                //add posting
                                var modelPosting = new Posting();
                                modelPosting.PostDate = today;
                                modelPosting.DesignID = design.DesignId;
                                modelPosting.CompetitionId = competitionId; //session registerCompetitionId
                                modelPosting.PostDescription = student.FirstName + " " + student.LastName + " joined the contest " + "\"" + comp.CompetitionName + "\""; //COMP NULL HERE IF MISSING COMPETTITION
                                modelPosting.StaffId = comp.StaffId;
                                db.Posting.Add(modelPosting);
                                db.SaveChanges();

                                //message box
                                string message = "File uploaded Successful";
                                TempData["message"] = "<script>alert('" + message + "');</script>";

                                newFilePath = Path.Combine("wwwroot/images/Medium", design.Painting); //new painting
                                await SendMailGoogleSmtp("phathuyhuukhanh@gmail.com", modelStaff.Email, "[" + student.StudentId + "]-[" + student.FirstName + " " + student.LastName + "] Has uploaded the drawing design", "<p><strong>PostingId:" + modelPosting.PostingId + "</strong></p><h3>DesignName: " + design.DesignName + "</h3><p>Description: " + design.Description + "</p><p>StudentName: " + student.FirstName + " " + student.LastName + "</p><br/>" + "", "phathuyhuukhanh@gmail.com", "t+NShmKmHyq0H7kp7ZBVRg==", newFilePath); //smtp gmail: phathuyhuukhanh@gmail.com                                

                                return RedirectToAction("Upload", "Home");
                            }
                            else if (file.Length > 8388608)
                            {
                                ViewBag.Msg = "Painting must be smaller than 8MB";
                            }
                            else
                            {
                                ViewBag.Msg = "Painting must be .jpg or .png";
                            }
                        }
                    }//check date
                    else
                    {
                        ViewBag.Msg = "Time to submit pictures for competition has expired ( Endate: " + comp.EndDate.ToString() + " )";
                    }
                }
                else
                {
                    ViewBag.Msg = "Model is invalid.";
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message;                
            }
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            string message = "";
            if (HttpContext.Session.GetString("studentid") == null) //check session
            {
                return RedirectToAction("Upload", "Home");
            }
            else
            {
                try
                {
                    var model = db.Design.SingleOrDefault(d => d.DesignId.Equals(id));
                    var modelPosting = db.Posting.SingleOrDefault(p => p.DesignID.Equals(model.DesignId));
                    var modelCompetition = db.Competition.SingleOrDefault(c => c.CompetitionId.Equals(modelPosting.CompetitionId));
                    var today = DateTime.Now;
                    ////check today SubmitDate                     
                    if (today >= modelCompetition.StartDate.Date && today <= modelCompetition.EndDate.Date)
                    {
                        if (modelPosting.Mark != null || modelPosting.Remark != null)
                        {
                            message = "This painting has been graded";
                        }
                        else
                        {
                            if (model != null)
                            {
                                string tempCurFilePath = Path.Combine("wwwroot/images/Medium", model.Painting); //old painting
                                db.Design.Remove(model);
                                db.SaveChanges();

                                message = "File deleted Successful";

                                //check old painting exists
                                System.GC.Collect();
                                System.GC.WaitForPendingFinalizers();
                                if (System.IO.File.Exists(tempCurFilePath))
                                {
                                    System.IO.File.Delete(tempCurFilePath);
                                }
                                TempData["message"] = "<script>alert('" + message + "');</script>";
                                return RedirectToAction("Upload", "Home");
                            }
                        }
                    }
                    else
                    {
                        return BadRequest("delete fail ((competition has expired))");
                    }
                }
                catch (Exception)
                {
                    return BadRequest("Delete Failed");
                }
            }
            //return View();
            TempData["message"] = "<script>alert('" + message + "');</script>";
            return RedirectToAction("Upload", "Home");
        }

        public interface IEmailService
        {
            void Send(string from, string to, string subject, string html);
        }

        private AlternateView getEmbeddedImage(String filePath, string _body)
        {
            // below line was corrected to include the mediatype so it displays in all 
            // mail clients. previous solution only displays in Gmail the inline images 
            LinkedResource res = new LinkedResource(filePath, MediaTypeNames.Image.Jpeg);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = @""+_body+"<br/><img src='cid:" + res.ContentId + @"'/>"; //html code here
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody,
             null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        //Send Mail Gmail SMTP
        public async Task<bool> SendMail(string _from, string _to, string _subject, string _body, SmtpClient client, string imageFilePath)
        {
            // Tạo nội dung Email
            MailMessage message = new MailMessage(
                from: _from,
                to: _to,
                subject: _subject,
                body: _body
            );
            //images
            message.AlternateViews.Add(getEmbeddedImage(imageFilePath, _body));
            //end images
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.ReplyToList.Add(new MailAddress(_from));
            message.Sender = new MailAddress(_from);


            try
            {
                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception /*ex*/)
            {
                throw;
                //Console.WriteLine(ex.Message);
                //return false;
            }
        }

        private async Task<bool> SendMailGoogleSmtp(string _from, string _to, string _subject, string _body, string _gmailsend, string _gmailpassword, string imageFilePath)
        {

            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            _gmailpassword = AesEncDesc.DecryptString(key, _gmailpassword);
            MailMessage message = new MailMessage(
                from: _from,
                to: _to,
                subject: _subject,
                body: _body               
            );
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            //images
            message.AlternateViews.Add(getEmbeddedImage(imageFilePath, _body));
            //end images
            message.ReplyToList.Add(new MailAddress(_from));
            message.Sender = new MailAddress(_from);

            //SmtpClient connect to smtp.gmail.com
            using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;
                client.Credentials = new NetworkCredential(_gmailsend, _gmailpassword);
                client.EnableSsl = true;
                return await SendMail(_from, _to, _subject, _body, client, imageFilePath);
            }

        }

        //LOGIN        
        public IActionResult Login()
        {
            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Login(StudentLogin studentLogin)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var model = db.Student.Where(s => s.StudentId.Equals(studentLogin.StudentId)).FirstOrDefault();
        //            if (model != null) //if found
        //            {
        //                var key = "b14ca5898a4e4133bbce2ea2315a1916";
        //                model.Password = AesEncDesc.DecryptString(key, model.Password);
        //                if (model.Password.Equals(studentLogin.Password))
        //                {
        //                    HttpContext.Session.SetString("studentid", studentLogin.StudentId); //session
        //                    return RedirectToAction("Index");
        //                }
        //                else
        //                {
        //                    ViewBag.Msg = "Wrong Password";
        //                }
        //            }
        //            else
        //            {
        //                ViewBag.Msg = "Username not found";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Msg = ex.Message;
        //    }
        //    return View();
        //}

        ////LOGOUT
        //public IActionResult Logout()
        //{
        //    var model = HttpContext.Session.GetString("studentid");
        //    if (model != null)
        //    {
        //        HttpContext.Session.Clear();
        //        return RedirectToAction("Login");

        //    }
        //    return View();
        //}
    }//end controller
}
