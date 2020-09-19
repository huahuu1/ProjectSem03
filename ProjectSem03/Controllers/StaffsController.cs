using System;
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

        [HttpGet]
        public IActionResult Create()
        {
            if(HttpContext.Session.GetString("staffRole") == "0")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Staffs");
            }
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
                        staff.ProfileImage = "../images/" + file.FileName;

                        var key = "b14ca5898a4e4133bbce2ea2315a1916";
                        staff.Password = AesEncDesc.EncryptString(key, staff.Password);
                        db.Staff.Add(staff);
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
            if (HttpContext.Session.GetString("staffRole") == "0")
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
            else
            {
                return RedirectToAction("Index", "Staffs");
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
                        editStaff.ProfileImage = staff.ProfileImage;

                        if (file.Length > 0)
                        {
                            string path = Path.Combine("wwwroot/images", file.FileName);
                            var stream = new FileStream(path, FileMode.Create);
                            file.CopyToAsync(stream);
                            editStaff.ProfileImage = "../images/" + file.FileName;

                            editStaff.Email = staff.Email;
                            editStaff.Phone = staff.Phone;
                            editStaff.Address = staff.Address;
                            db.SaveChanges();
                            return RedirectToAction("Index", "Staffs");
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "Failed .......";
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
