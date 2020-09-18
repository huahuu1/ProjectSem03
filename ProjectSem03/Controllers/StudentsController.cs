using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ProjectSem03.Controllers
{
    public class StudentsController : Controller
    {
        ProjectDB db;
        public StudentsController(ProjectDB db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("staffId") == null) //check session
            {
                return RedirectToAction("Login");
            }
            else
            {
                //IEnumerable<Student> list = db.Student.ToList();
                var list = db.Student.ToList();
                return View(list);
            }
        }

        //Method
        private string GenId()
        {
            var model = db.Student.Last();
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
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("staffId") == null) //check login ROLE MANAGER
            {
                return RedirectToAction("Login");
            }
            else
            {                
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student, IFormFile file)
        {
            try
            {
                if (ModelState.IsValid && file != null) //check CreateViewStudent and profileimages
                {
                    if (file.Length > 0)
                    {
                        //profile images must be .jpg or .png
                        if (Path.GetExtension(file.FileName).ToLower().Equals(".jpg") || Path.GetExtension(file.FileName).ToLower().Equals(".png"))
                        {
                            //choose image
                            string path = Path.Combine("wwwroot/images", file.FileName);
                            var stream = new FileStream(path, FileMode.Create);
                            file.CopyToAsync(stream);
                            student.ProfileImage = "../images/" + file.FileName;
                            //key
                            var key = "b14ca5898a4e4133bbce2ea2315a1916";
                            student.Password = AesEncDesc.EncryptString(key, student.Password);
                            student.StudentId = GenId();
                            db.Student.Add(student);
                            db.SaveChanges();
                            stream.Close();
                            return RedirectToAction("Index", "Students");
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "Profile images must be .jpg or .png";
                        return View();
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
        public IActionResult Edit(string id)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Student student, IFormFile file)
        {
            try
            {
                var model = db.Student.SingleOrDefault(s => s.StudentId.Equals(student.StudentId));
                if (ModelState.IsValid)
                {
                    if (model != null)
                    {
                        if (file != null && file.Length > 0) //profile images must be .jpg
                        {
                            if (Path.GetExtension(file.FileName).ToLower().Equals(".jpg") || Path.GetExtension(file.FileName).ToLower().Equals(".png"))
                            {
                                string path = Path.Combine("wwwroot/images", file.FileName);
                                var stream = new FileStream(path, FileMode.Create);
                                file.CopyToAsync(stream);
                                model.FirstName = student.FirstName;
                                model.LastName = student.LastName;
                                model.DateOfBirth = student.DateOfBirth;
                                model.Gender = student.Gender;
                                model.Phone = student.Phone;
                                model.Email = student.Email;
                                model.JoinDate = student.JoinDate;
                                model.Address = student.Address;
                                student.ProfileImage = "../images/" + file.FileName;
                                model.ProfileImage = student.ProfileImage;
                                //Staff cannot change Student CompetitionId and Password
                                db.SaveChanges();
                                stream.Close();
                                return RedirectToAction("Index", "Students");
                            }
                        }
                        else if (file == null) //if no change profile images
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
                            ViewBag.Msg = "Profile images must be .jpg or .png";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "Failed";
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message;
            }
            ViewBag.Msg = "Update Failed";
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
                        db.Student.Remove(model);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Students");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return View();
            } //end check session
        }
    }
}
