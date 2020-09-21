using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartBreadcrumbs.Attributes;
using X.PagedList;

namespace ProjectSem03.Controllers
{
    public class StaffsController : Controller
    {
        ProjectDB db;
        public StaffsController(ProjectDB db)
        {
            this.db = db;
        }

        [Breadcrumb("Staff List")]
        public IActionResult Index(string sname, int? page)
        {
            if (HttpContext.Session.GetString("staffId") == null) //check session
            {
                return RedirectToAction("Login");
            }
            else
            {
                int maxsize = 3;
                int numpage = page ?? 1;
                var list = db.Staff.ToList().ToPagedList(numpage, maxsize);
                if (string.IsNullOrEmpty(sname))
                {
                    ViewBag.page = list;
                    //return View(list);
                }
                else
                {
                    list = list.Where(s => s.StaffName.Contains(sname)).ToList().ToPagedList(numpage, maxsize);
                    ViewBag.page = list;
                    //return View(filter);
                }
                return View();
            }

        }

        [HttpGet]
        [Breadcrumb("Create Staff")]
        public IActionResult Create()
        {
            if(HttpContext.Session.GetInt32("staffRole") == 0)
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
            var staffid = db.Staff.SingleOrDefault(s => s.StaffId.Equals(staff.StaffId));
            var email = db.Staff.SingleOrDefault(e => e.Email.Equals(staff.Email));
            var phone = db.Staff.SingleOrDefault(p => p.Phone.Equals(staff.Phone));

            try
            {
                if (ModelState.IsValid)
                {
                    if (file!= null && file.Length > 0)
                    {
                        string path = Path.Combine("wwwroot/images", file.FileName);
                        var stream = new FileStream(path, FileMode.Create);
                        file.CopyToAsync(stream);
                        staff.ProfileImage = "/images/teachers/" + file.FileName;

                        var key = "b14ca5898a4e4133bbce2ea2315a1916";
                        staff.Password = AesEncDesc.EncryptString(key, staff.Password);

                        if(staffid == null)
                        {
                            if (email == null)
                            {
                                if (phone == null)
                                {
                                    db.Staff.Add(staff);
                                    stream.Close();
                                    db.SaveChanges();
                                    return RedirectToAction("Index", "Staffs");
                                }
                                else
                                {
                                    ViewBag.Phone = "Phone is already existed. Try again";
                                }
                            }
                            else
                            {
                                ViewBag.Email = "Email is already existed. Try again";
                            }
                        }
                        else
                        {
                            ViewBag.StaffId = "StaffId is already existed. Try again";
                        }
                    }
                    else
                    {
                        ViewBag.Image = "Image Upload Container Cannot Be Empty";
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

        [HttpGet]
        [Breadcrumb("Edit Staff")]
        public IActionResult Edit(string id)
        {
            if (HttpContext.Session.GetInt32("staffRole") == 0)
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
