using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Http;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using X.PagedList; //using for pagination

namespace ProjectSem03.Controllers
{
    public class StudentsController : Controller
    {
        //connection to database
        ProjectDB db;
        public StudentsController(ProjectDB db)
        {
            this.db = db;
        }

        [Breadcrumb("Student List")]
        public IActionResult Index(string sname, int? page)
        {
            if (HttpContext.Session.GetString("staffId") == null) //check session
            {
                return RedirectToAction("Login");
            }
            else
            {
                //set number of records per page and starting page
                int maxsize = 3;
                int numpage = page ?? 1;

                var list = db.Student.ToList().ToPagedList(numpage, maxsize); //get list of students and pagination

                //check if result is found or not
                if (string.IsNullOrEmpty(sname)) //empty
                {
                    ViewBag.page = list;
                }
                else
                {
                    //show the result
                    list = db.Student.Where(s => s.FirstName.ToLower().Contains(sname) || s.FirstName.ToUpper().Contains(sname) || s.LastName.ToLower().Contains(sname) || s.LastName.ToUpper().Contains(sname)).ToList().ToPagedList(numpage, maxsize);
                    ViewBag.page = list;
                }
                return View();
            }
        }

        //Method
        private string GenId() //automic generate Student Id
        {
            //var model = db.Student.LastOrDefault();
            var model = (from s in db.Student orderby s.StudentId descending select s).First();
            string FirstId = model.StudentId.Substring(0, 3);
            string AffterID = model.StudentId.Substring(3, 7);

            string LastNummberId = "";
            for (int i = 0; i <= AffterID.Length - 1; i++)
            {
                if (int.Parse(AffterID[i].ToString()) != 0)
                {
                    LastNummberId = AffterID.Substring(i, AffterID.Length - i);
                    break;
                }
            }

            LastNummberId = (Convert.ToInt32(LastNummberId) + 1).ToString();

            int CountId = LastNummberId.Length; //full lenghtid
            Console.WriteLine(CountId);
            string FullId = FirstId;
            for (int i = 0; i < 7; i++)
            {
                if (i == 7 - CountId)
                {
                    FullId += LastNummberId;
                    break;
                }
                else
                {
                    FullId += "0";
                }
            }

            return FullId;
        }

        //CREATE
        [HttpGet]
        [Breadcrumb("Create Student")]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("staffRole") == 0)
            {
                if (HttpContext.Session.GetString("staffId") == null) //check login
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                //return to Index page of Staffs
                return RedirectToAction("Index", "Staffs");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 8388608)]
        [RequestSizeLimit(8388608)]
        public async Task<IActionResult> Create(Student student, IFormFile file, [FromServices] IWebHostEnvironment owebHostEnvironment) //create new Student
        {
            try
            {
                if (ModelState.IsValid) //check CreateViewStudent and profileimages
                {
                    if (file == null)
                    {
                        ViewBag.Msg = "Profile images is required";
                    }
                    else {
                        string ext = Path.GetExtension(file.FileName);
                        var today = DateTime.Now;

                        //profile images must be .jpg
                        if ((file.Length > 0 && file.Length < 8388608) && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".png")))
                        {
                            student.StudentId = GenId();
                            //choose image
                            string renameFile = Convert.ToString(Guid.NewGuid()) + today.ToString("_yyyy-MM-dd_hhmmss_tt") + ext;
                            string fileNameAndPath = $"{owebHostEnvironment.WebRootPath}\\images\\students\\{renameFile}";
                            using (var stream = new FileStream(fileNameAndPath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                                await stream.FlushAsync();
                            }
                            student.ProfileImage = "../images/students/"+renameFile;
                            //key
                            var key = "b14ca5898a4e4133bbce2ea2315a1916";
                            student.Password = AesEncDesc.EncryptString(key, student.Password);
                            db.Student.Add(student);
                            db.SaveChanges();
                            return RedirectToAction("Index", "Students");
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
                {
                    ViewBag.Msg = "Failed";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Msg = ex.Message;
            }
            return View();
        }

        //EDIT
        [HttpGet]
        [Breadcrumb("Edit Student")]
        public IActionResult Edit(string id)
        {
            if (HttpContext.Session.GetInt32("staffRole") == 0)
            {
                if (HttpContext.Session.GetString("staffId") == null) //check session
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    var model = db.Student.Find(id);
                    if (model != null)
                    {
                        return View(model); ;
                    }
                    else
                    {
                        return View();
                    }
                } //end check session
            }
            else
            {
                return RedirectToAction("Index", "Staffs");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 8388608)]
        [RequestSizeLimit(8388608)]
        public async Task<IActionResult> Edit(Student student, IFormFile file, [FromServices] IWebHostEnvironment owebHostEnvironment)
        {
            try
            {
                var model = db.Student.SingleOrDefault(s => s.StudentId.Equals(student.StudentId));
                if (ModelState.IsValid)
                {
                    if (model != null)
                    {
                        if (file == null) //if no change profile images
                        {
                            model.FirstName = student.FirstName;
                            model.LastName = student.LastName;
                            model.DateOfBirth = student.DateOfBirth;
                            model.Gender = student.Gender;
                            model.Phone = student.Phone;
                            model.Email = student.Email;
                            model.JoinDate = student.JoinDate;
                            model.Address = student.Address;
                            db.SaveChanges();
                            return RedirectToAction("Index", "Students");
                        }
                        else
                        {
                            string ext = Path.GetExtension(file.FileName);
                            var today = DateTime.Now;

                            if ((file.Length > 0 && file.Length < 8388608) && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".png"))) //painting must be .jpg or .png
                            {
                                string tempCurFilePath = Path.Combine("wwwroot/", model.ProfileImage.Substring(3)); //old painting
                                string renameFile = Convert.ToString(Guid.NewGuid()) + today.ToString("_yyyy-MM-dd_hhmmss_tt") + ext;
                                string fileNameAndPath = $"{owebHostEnvironment.WebRootPath}\\images\\students\\{renameFile}";

                                using (var stream = new FileStream(fileNameAndPath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                    await stream.FlushAsync();
                                }
                                model.FirstName = student.FirstName;
                                model.LastName = student.LastName;
                                model.DateOfBirth = student.DateOfBirth;
                                model.Gender = student.Gender;
                                model.Phone = student.Phone;
                                model.Email = student.Email;
                                model.JoinDate = student.JoinDate;
                                model.Address = student.Address;
                                student.ProfileImage = "../images/students/"+renameFile;
                                model.ProfileImage = student.ProfileImage;
                                //Staff cannot change Student CompetitionId and Password
                                //stream.Close();
                                db.SaveChanges();

                                System.GC.Collect();
                                System.GC.WaitForPendingFinalizers();
                                //check old painting exists
                                if (System.IO.File.Exists(tempCurFilePath))
                                {
                                    System.IO.File.Delete(tempCurFilePath);
                                }
                                //messagebox
                                string message = "Student updated Successful";
                                TempData["message"] = "<script>alert('" + message + "');</script>";

                                return RedirectToAction("Index", "Students");
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
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message;
            }
            //ViewBag.Msg = "Update Failed";
            return View();
        }

        //DELETE
        public IActionResult Delete(string id)
        {
            if (HttpContext.Session.GetString("staffId") == null) //check session
            {
                return RedirectToAction("Login");
            }
            else
            {
                try
                {
                    var model = db.Student.SingleOrDefault(s => s.StudentId.Equals(id));

                    if (model != null)
                    {
                        string tempCurFilePath = Path.Combine("wwwroot/", model.ProfileImage.Substring(3)); //old painting
                        db.Student.Remove(model);
                        db.SaveChanges();
                        //check old painting exists
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                        if (System.IO.File.Exists(tempCurFilePath))
                        {
                            System.IO.File.Delete(tempCurFilePath);
                        }
                        TempData["message"] = "<script>alert('Students deleted Successful');</script>";
                        return RedirectToAction("Index", "Students");
                    }
                }
                catch (Exception)
                {
                    return BadRequest("Delete Failed");
                }
                TempData["message"] = "<script>alert('Delete fail');</script>";
                return View();
            } //end check session
        }
    }
}
