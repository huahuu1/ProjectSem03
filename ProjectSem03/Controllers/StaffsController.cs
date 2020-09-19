﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectSem03.Controllers
{
    public class StaffsController : Controller
    {
        ProjectDB db;
        public StaffsController(ProjectDB db)
        {
            this.db = db;
        }

        public IActionResult Index(string sname)
        {
            if (HttpContext.Session.GetString("staffId") == null) //check session
            {
                return RedirectToAction("Login");
            }
            else
            {
                var list = db.Staff.ToList();
                if (string.IsNullOrEmpty(sname))
                {
                    return View(list);
                }
                else
                {
                    var filter = list.Where(s => s.StaffName.ToLower().Contains(sname) || s.StaffName.ToUpper().Contains(sname));
                    return View(filter);
                }
            }
            
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ActionName("Create")]
        public IActionResult Create(Staff staff, IFormFile file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (file.Length > 0)
                    {
                        string path = Path.Combine("wwwroot/images", file.FileName);
                        var stream = new FileStream(path, FileMode.Create);
                        file.CopyToAsync(stream);
                        staff.ProfileImage = "/images/teachers/" + file.FileName;

                        var key = "b14ca5898a4e4133bbce2ea2315a1916";
                        staff.Password = AesEncDesc.EncryptString(key, staff.Password);
                        db.Staff.Add(staff);
                        stream.Close();
                        db.SaveChanges();
                        return RedirectToAction("Index", "Staffs");
                    }
                }
                else
                {
                    ViewBag.Msg = "Failed .......";
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message;
            }
            return View();
        }

        public IActionResult Edit(string id)
        {
            var stf = db.Staff.Find(id);
            if (stf != null)
            {
                return View(stf);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(Staff staff, IFormFile file)
        {
            try
            {
                var editStaff = db.Staff.SingleOrDefault(c => c.StaffId.Equals(staff.StaffId));
                if (ModelState.IsValid)
                {
                    if (editStaff != null)
                    {
                        if (file != null && file.Length > 0) //profile images must be .jpg
                        {
                            if (Path.GetExtension(file.FileName).ToLower().Equals(".jpg"))
                            {
                                string path = Path.Combine("wwwroot/images", file.FileName);
                                var stream = new FileStream(path, FileMode.Create);
                                file.CopyToAsync(stream);
                                editStaff.ProfileImage = "/images/teachers/" + file.FileName;

                                editStaff.Email = staff.Email;
                                editStaff.Phone = staff.Phone;
                                editStaff.Address = staff.Address;
                                stream.Close();
                                db.SaveChanges();
                                return RedirectToAction("Index", "Staffs");
                            }
                        }
                        else if (file == null) //if no change profile images
                        {
                            editStaff.Email = staff.Email;
                            editStaff.Phone = staff.Phone;
                            editStaff.Address = staff.Address;
                            db.SaveChanges();
                            return RedirectToAction("Index", "Staffs");
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
            return View();
        }

        public IActionResult Delete(string id)
        {
            try
            {
                var staff = db.Staff.SingleOrDefault(s => s.StaffId.Equals(id));

                if (staff != null)
                {
                    db.Staff.Remove(staff);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Staffs");
                }
            }
            catch (Exception)
            {
                return BadRequest("Cannot delete object that is existed in other tables");
            }
            return View();
        }
    }
}
