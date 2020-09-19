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
                var list = db.Student.ToList();
                return View(list);
            }
        }

        //CREATE
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
                return RedirectToAction("Index", "Staffs");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Student student, IFormFile file)
        {
            try
            {
                if (ModelState.IsValid) //check CreateViewStudent and profileimages
                {
                    if (file != null && file.Length > 0)
                    {
                        //profile images must be .jpg
                        if (Path.GetExtension(file.FileName).ToLower().Equals(".jpg"))
                        {
                            //choose image
                            string path = Path.Combine("wwwroot/images", file.FileName);
                            var stream = new FileStream(path, FileMode.Create);
                            file.CopyToAsync(stream);
                            student.ProfileImage = "/images/" + file.FileName;
                            //key
                            var key = "b14ca5898a4e4133bbce2ea2315a1916";
                            student.Password = AesEncDesc.EncryptString(key, student.Password);
                            db.Student.Add(student);
                            db.SaveChanges();
                            stream.Close();
                            return RedirectToAction("Index", "Students");
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "Profile images must be .jpg";
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
                            if (Path.GetExtension(file.FileName).ToLower().Equals(".jpg"))
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
                                student.ProfileImage = "/images/" + file.FileName;
                                model.ProfileImage = student.ProfileImage;
                                //Staff cannot change Student CompetitionId and Password
                                db.SaveChanges();
                                
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
                            ViewBag.Msg = "Profile images must be .jpg";
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
