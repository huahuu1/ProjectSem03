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
using X.PagedList; //using for pagination

namespace ProjectSem03.Controllers
{
    public class StaffsController : Controller
    {
        //connection to database
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
                //set number of records per page and starting page
                int maxsize = 3;
                int numpage = page ?? 1;

                var list = db.Staff.ToList().ToPagedList(numpage, maxsize); //get list of Staffs and pagination

                //check if result is found or not
                if (string.IsNullOrEmpty(sname))//empty
                {
                    ViewBag.page = list;
                }
                else
                {
                    //show the result
                    list = db.Staff.Where(s => s.StaffName.ToLower().Contains(sname)).ToList().ToPagedList(numpage, maxsize);
                    ViewBag.page = list;
                }
                return View();
            }

        }

        [HttpGet]
        [Breadcrumb("Create Staff")]
        public IActionResult Create()
        {
            if(HttpContext.Session.GetInt32("staffRole") == 0) //check session for Staff Role
            {
                return View();
            }
            else //if Staff Role doesn't equal 0
            {
                //return to Index page of Staffs
                return RedirectToAction("Index", "Staffs");
            }
        }
        [HttpPost]
        [ActionName("Create")]
        public IActionResult Create(Staff staff, IFormFile file) //create new Staff
        {
            var staffid = db.Staff.SingleOrDefault(s => s.StaffId.Equals(staff.StaffId)); //check if input Staff Id is existed in database or not
            var email = db.Staff.SingleOrDefault(e => e.Email.Equals(staff.Email)); //check if input Email is existed in database or not
            var phone = db.Staff.SingleOrDefault(p => p.Phone.Equals(staff.Phone));//check if input Phone Id is existed in database or not

            try
            {
                if (ModelState.IsValid)
                {
                    //check conditions of creating
                    if (file!= null && file.Length > 0) //if image is chosen
                    {
                        string path = Path.Combine("wwwroot/images", file.FileName); //destination of image source
                        var stream = new FileStream(path, FileMode.Create); //create file stream
                        file.CopyToAsync(stream); //copy image file
                        staff.ProfileImage = "/images/teachers/" + file.FileName; //set copied image to Staff ProfileImage

                        var key = "b14ca5898a4e4133bbce2ea2315a1916"; //encrypting key
                        staff.Password = AesEncDesc.EncryptString(key, staff.Password); //ecrypt password

                        if(staffid == null) //if Staff Id doesn't exist
                        {
                            if (email == null) //if Email doesn't exist
                            {
                                if (phone == null) //if Phone doesn't exist
                                {
                                    db.Staff.Add(staff); //add new staff
                                    stream.Close(); //close stream
                                    db.SaveChanges(); //save changes
                                    return RedirectToAction("Index", "Staffs");
                                }
                                else
                                {
                                    ViewBag.Phone = "Phone is already existed. Try again"; //error message for phone
                                }
                            }
                            else
                            {
                                ViewBag.Email = "Email is already existed. Try again"; //error message for Email
                            }
                        }
                        else
                        {
                            ViewBag.StaffId = "StaffId is already existed. Try again"; //error message for StaffId
                        }
                    }
                    else
                    {
                        ViewBag.Image = "Image Upload Container Cannot Be Empty"; //error message for Image
                    }
                }
                else
                {
                    ViewBag.Msg = "Failed ......."; // show error message if conditions are invalid
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message; //show other error messages
            }
            return View();
        }

        [HttpGet]
        [Breadcrumb("Edit Staff")]
        public IActionResult Edit(string id)
        {
            if (HttpContext.Session.GetInt32("staffRole") == 0)
            {
                var stf = db.Staff.Find(id); //Find Staff Id
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
        public IActionResult Edit(Staff staff, IFormFile file) //edit Staff
        {
            try
            {
                var editStaff = db.Staff.SingleOrDefault(c => c.StaffId.Equals(staff.StaffId)); //check Staff Id
                if (ModelState.IsValid)
                {
                    //check conditions of editting
                    if (editStaff != null)
                    {
                        if (file != null && file.Length > 0) //profile images must be .jpg
                        {
                            if (Path.GetExtension(file.FileName).ToLower().Equals(".jpg")) //check image extension
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
                            ViewBag.Msg = "Profile images must be .jpg"; //image extension error message
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

        public IActionResult Delete(string id) //delete staff
        {
            try
            {
                var staff = db.Staff.SingleOrDefault(s => s.StaffId.Equals(id)); //find staff id

                if (staff != null)
                {
                    db.Staff.Remove(staff); //remove chosen Staff from database
                    db.SaveChanges();
                    return RedirectToAction("Index", "Staffs");
                }
            }
            catch (Exception)
            {
                return BadRequest("Cannot delete object that is existed in other tables"); //error message if chosen Staff found in other databases
            }
            return View();
        }
    }
}
