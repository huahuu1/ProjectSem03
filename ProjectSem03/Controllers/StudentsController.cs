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
using Microsoft.CodeAnalysis.FlowAnalysis;
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
                int maxsize = 5;
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
                    list = db.Student.Where(s => s.FirstName.ToLower().Contains(sname) || s.FirstName.ToUpper().Contains(sname) || s.LastName.ToLower().Contains(sname) || s.LastName.ToUpper().Contains(sname)).ToList().ToPagedList();
                    ViewBag.page = list;
                }
                return View();
            }
        }

        //Method
        private string GenId()
        {            
            var modelcheck = db.Student.ToList(); //get model Student

            bool check = false;
            string FirstId = "";
            string AffterID = "";
            //check if database <= 0
            if (modelcheck.Count <= 0)
            {
                FirstId = "STU";
                AffterID = "0000001";
                check = true;
            }            
            else
            {
                var model = (from s in db.Student orderby s.StudentId descending select s).First(); //Get Last Id
                FirstId = model.StudentId.Substring(0, 3);
                AffterID = model.StudentId.Substring(3, 7);
            }
            string LastNummberId = "";
            for (int i = 0; i <= AffterID.Length - 1; i++)
            {
                if (int.Parse(AffterID[i].ToString()) != 0)
                {
                    LastNummberId = AffterID.Substring(i, AffterID.Length - i); //get all id where != 0                    
                    break;
                }
            }
            //if row > 0
            if (check == false)
            {
                LastNummberId = (Convert.ToInt32(LastNummberId) + 1).ToString(); //id number + 1 to string
            }
            else
            {
                LastNummberId = (Convert.ToInt32(LastNummberId)).ToString();
            }

            int CountId = LastNummberId.Length; //full lenght id
            //Console.WriteLine(CountId);
            string FullId = FirstId;
            for (int i = 0; i < 7; i++)
            {
                if (i == 7 - CountId)
                {
                    FullId += LastNummberId; //valid
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
                    var mEmail = db.Student.SingleOrDefault(s=>s.Email.Equals(student.Email));
                    var mPhone = db.Student.SingleOrDefault(s => s.Phone.Equals(student.Phone));
                    //valid
                    bool checkOk = true;
                    //check unique email phone
                    if (mEmail != null || mPhone != null)
                    {
                        if (mEmail != null)
                        {
                            ViewBag.Email = "Email is already existed. Try again";
                        }
                        if(mPhone != null)
                        {
                            ViewBag.Phone = "Phone is already existed. Try again";
                        }
                        checkOk = false;
                    }
                        if (file != null)
                        {
                            string ext = Path.GetExtension(file.FileName);
                            var today = DateTime.Now;

                            //profile images must be .jpg or .png
                            if ((file.Length > 0 && file.Length < 8388608) && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".png")))
                            {
                                //check unique email or phone
                                if (checkOk == false)
                                {
                                    return View();
                                }
                                //auto generated id
                                student.StudentId = GenId();
                                //generate images name choose image path
                                string renameFile = Convert.ToString(Guid.NewGuid()) + today.ToString("_yyyy-MM-dd_hhmmss_tt") + ext;
                                string fileNameAndPath = $"{owebHostEnvironment.WebRootPath}\\images\\students\\{renameFile}";
                                using (var stream = new FileStream(fileNameAndPath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                    await stream.FlushAsync();
                                }
                                student.ProfileImage = "/images/students/" + renameFile;
                                //key
                                var key = "b14ca5898a4e4133bbce2ea2315a1916";
                                student.Password = AesEncDesc.EncryptString(key, student.Password);
                                db.Student.Add(student);
                                db.SaveChanges();
                                return RedirectToAction("Index", "Students");
                            }
                            else if (file.Length > 8388608)
                            {
                                ViewBag.PImage = "ProfileImage must be smaller than 8MB";
                            }
                            else
                            {
                                ViewBag.PImage = "ProfileImage must be .jpg or .png";
                            }
                        }//end check file not null
                        else
                        {
                            ViewBag.PImage = "Profile images is required";
                        }
                }
                else
                {
                    ViewBag.Msg = "Invalid Field";
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
            //try
            //{
                var model = db.Student.SingleOrDefault(s => s.StudentId.Equals(student.StudentId));                
                if (ModelState.IsValid)
                {
                    if (model != null)
                    {
                        //valid
                        bool checkOk = true;
                        //check unique phone and email
                        var mEmail = db.Student.SingleOrDefault(s => s.Email.Equals(student.Email) && s.Email != model.Email);
                        var mPhone = db.Student.SingleOrDefault(s => s.Phone.Equals(student.Phone) && s.Phone != model.Phone);
                        if (mEmail != null || mPhone != null)
                        {

                            if (mEmail != null)
                            {
                                ViewBag.Email = "Email is already existed. Try again";
                            }
                            if (mPhone != null)
                            {
                                ViewBag.Phone = "Phone is already existed. Try again";
                            }
                            checkOk = false;
                        }
                        else
                        {
                            //if no change profile images
                            if (file == null)
                            {
                                
                                //check unique email or phone
                                if(checkOk == false)
                                {
                                    ViewBag.Msg = "Fail";
                                    return View();
                                }
                                //key
                                var key = "b14ca5898a4e4133bbce2ea2315a1916";
                                student.Password = AesEncDesc.EncryptString(key, student.Password);
                                model.Password = student.Password;
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

                                //get file .Extension
                                string ext = Path.GetExtension(file.FileName);
                                var today = DateTime.Now;

                                if ((file.Length > 0 && file.Length < 8388608) && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".png"))) //painting must be .jpg or .png
                                {
                                    //check unique email or phone
                                    if(checkOk == false)
                                    {
                                        ViewBag.Msg = "Fail";
                                        return View();
                                    }
                                    string tempCurFilePath = Path.Combine("wwwroot/", model.ProfileImage.Substring(1)); //old painting
                                    
                                    //auto generate filename
                                    string renameFile = Convert.ToString(Guid.NewGuid()) + today.ToString("_yyyy-MM-dd_hhmmss_tt") + ext;
                                    string fileNameAndPath = $"{owebHostEnvironment.WebRootPath}\\images\\students\\{renameFile}";
                                    
                                    //Copy to file path
                                    using (var stream = new FileStream(fileNameAndPath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                        await stream.FlushAsync();
                                    }

                                    //key
                                    var key = "b14ca5898a4e4133bbce2ea2315a1916";
                                    student.Password = AesEncDesc.EncryptString(key, student.Password);

                                    model.Password = student.Password;
                                    model.FirstName = student.FirstName;
                                    model.LastName = student.LastName;
                                    model.DateOfBirth = student.DateOfBirth;
                                    model.Gender = student.Gender;
                                    model.Phone = student.Phone;
                                    model.Email = student.Email;
                                    model.JoinDate = student.JoinDate;
                                    model.Address = student.Address;
                                    student.ProfileImage = "/images/students/" + renameFile;
                                    model.ProfileImage = student.ProfileImage;
                                    //Staff cannot change Student CompetitionId and Password
                                    db.SaveChanges();

                                    System.GC.Collect();
                                    System.GC.WaitForPendingFinalizers();
                                    //check old painting exists to delete
                                    if (System.IO.File.Exists(tempCurFilePath))
                                    {
                                        System.IO.File.Delete(tempCurFilePath);
                                    }
                                    //messagebox
                                    string message = "Student updated Successful";
                                    TempData["message"] = "<script>alert('" + message + "');</script>";

                                    return RedirectToAction("Index", "Students");
                                }
                                else if (file.Length > 8388608) //check file capacity
                                {
                                    ViewBag.PImage = "Painting must be smaller than 8MB";
                                }
                                else
                                {
                                    ViewBag.PImage = "Painting must be .jpg or .png";
                                }
                            }//end check file !null
                        }//end check unique phone email
                    }//end check model !null
                }//end check model valid
                else
                {
                    ViewBag.Msg = "Invalid Field";
                }
            //}
            //catch (Exception e)
            //{
            //    ViewBag.Msg = e.Message;
            //}
            //ViewBag.Msg = "Update Failed";
            return View();
        }

        //DELETE
        public IActionResult Delete(string id)
        {
            if (HttpContext.Session.GetInt32("staffRole") == 0)
            {
                if (HttpContext.Session.GetString("staffId") == null) //check session
                {
                    return RedirectToAction("Index", "Staffs");
                }
                else
                {
                    try
                    {
                        var model = db.Student.SingleOrDefault(s => s.StudentId.Equals(id));

                        if (model != null)
                        {
                            string tempCurFilePath = Path.Combine("wwwroot/", model.ProfileImage.Substring(1)); //old painting
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
            else
            {
                return RedirectToAction("Index", "Staffs");
            }
        }
    }
}
