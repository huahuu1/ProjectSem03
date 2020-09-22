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
using Microsoft.AspNetCore.Hosting;

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
                int maxsize = 5;
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
                    list = db.Staff.Where(s => s.StaffName.ToLower().Contains(sname)).ToList().ToPagedList();
                    ViewBag.page = list;
                }
                return View();
            }

        }
        [HttpGet]
        [Breadcrumb("Create Staff")]
        public IActionResult Create()
        {
            //role administrator
            if(HttpContext.Session.GetInt32("staffRole") == 0)
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
        [RequestFormLimits(MultipartBodyLengthLimit = 8388608)]
        [RequestSizeLimit(8388608)]
        public async Task<IActionResult> Create(Staff staff, IFormFile file, [FromServices] IWebHostEnvironment owebHostEnvironment)
        {
            var staffid = db.Staff.SingleOrDefault(s => s.StaffId.Equals(staff.StaffId)); //check if input Staff Id is existed in database or not
            var email = db.Staff.SingleOrDefault(e => e.Email.Equals(staff.Email)); //check if input Email is existed in database or not
            var phone = db.Staff.SingleOrDefault(p => p.Phone.Equals(staff.Phone));//check if input Phone Id is existed in database or not

            try
            {
                if (ModelState.IsValid)
                {
                    //valid
                    bool checkOk = true;
                    //check unique email phone staff id
                    if (email != null || phone != null || staffid !=null)
                    {
                        if (email != null)
                        {
                            ViewBag.Email = "Email is already existed. Try again";
                        }
                        if (phone != null)
                        {
                            ViewBag.Phone = "Phone is already existed. Try again";
                        }
                        if(staffid != null)
                        {
                            ViewBag.StaffId = "StaffId is already existed. Try again";
                        }
                        checkOk = false;
                    }

                    //check file not null
                    if (file!= null && file.Length > 0)
                    {

                        //check picture duplicate
                        var modelDuplicate = db.Staff.SingleOrDefault(s => s.ProfileImage.Equals("/images/teachers/" + file.FileName));
                        //if (modelDuplicate != null)
                        //{
                            if (modelDuplicate != null)
                            {
                                ViewBag.Painting = "File name already exists";
                                checkOk = false;
                            }
                        //    checkOk = false;
                        //}

                        //get file .Extensition
                        string ext = Path.GetExtension(file.FileName);
                        if ((file.Length > 0 && file.Length < 8388608) && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".png")))
                        {

                            //check picture duplicate, unique email or phone staffid
                            if (checkOk == false)
                            {
                                return View();
                            }

                            //choose image path
                            string fileNameAndPath = $"{owebHostEnvironment.WebRootPath}\\images\\teachers\\{file.FileName}";
                            using (var stream = new FileStream(fileNameAndPath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                                await stream.FlushAsync();
                            }

                            staff.ProfileImage = "/images/teachers/" + file.FileName;

                            //Encrypt Password
                            var key = "b14ca5898a4e4133bbce2ea2315a1916";
                            staff.Password = AesEncDesc.EncryptString(key, staff.Password);

                            db.Staff.Add(staff);
                            db.SaveChanges();
                            return RedirectToAction("Index", "Staffs");
                        }
                        else if (file.Length > 8388608) //check images capacity
                        {
                            ViewBag.PImages = "Profile Images must be smaller than 8MB";
                        }
                        else
                        {
                            ViewBag.PImages = "Profile Images must be .jpg or .png";
                        }
                    }
                    else
                    {
                        ViewBag.PImages = "Image Upload Container Cannot Be Empty";
                    }
                }
                else
                {
                    ViewBag.Msg = "Failed .......";
                }//end check file lenght
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
        [RequestFormLimits(MultipartBodyLengthLimit = 8388608)]
        [RequestSizeLimit(8388608)]
        public async Task<IActionResult> Edit(Staff staff, IFormFile file, [FromServices] IWebHostEnvironment owebHostEnvironment)
        {
            try
            {
                var editStaff = db.Staff.SingleOrDefault(c => c.StaffId.Equals(staff.StaffId)); //check Staff Id
                if (ModelState.IsValid)
                {
                    //check conditions of editting
                    if (editStaff != null)
                    {
                        //valid
                        bool checkOk = true;
                        //check unique phone and email
                        var mEmail = db.Staff.SingleOrDefault(s => s.Email.Equals(staff.Email) && s.Email != editStaff.Email);
                        var mPhone = db.Staff.SingleOrDefault(s => s.Phone.Equals(staff.Phone) && s.Phone != editStaff.Phone);
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

                        //if no change profile images
                        if (file == null)
                            {
                                //check unique email or phone
                                if(checkOk == false)
                                {
                                    ViewBag.Msg = "Fail";
                                    return View();
                                }
                                editStaff.Email = staff.Email;
                                editStaff.Phone = staff.Phone;
                                editStaff.Address = staff.Address;
                                db.SaveChanges();
                                return RedirectToAction("Index", "Staffs");
                            }
                            else
                            {

                                //check picture duplicate
                                var modelDuplicate = db.Staff.SingleOrDefault(s => s.ProfileImage.Equals("/images/teachers/"+file.FileName) && s.ProfileImage != editStaff.ProfileImage);
                                //if (modelDuplicate != null)
                                //{
                                    if(modelDuplicate != null)
                                    {
                                        ViewBag.PImages = "File name already exists";
                                        checkOk = false;
                                    }
                                    //checkOk = false;
                                //}

                                //check file
                                string ext = Path.GetExtension(file.FileName);
                                var today = DateTime.Now;
                                if ((file.Length > 0 && file.Length < 8388608) && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".png"))) //painting must be .jpg or .png
                                {

                                        //check picture duplicate, unique email or phone
                                        if (checkOk == false)
                                        {
                                            return View();
                                        }

                                        //check if new file equal current file
                                        bool checkNotDelete = false;
                                        string tempCurFilePath = Path.Combine("wwwroot/", editStaff.ProfileImage.Substring(1)); //old painting
                                        if (("/images/" + file.FileName).Equals(editStaff.ProfileImage))
                                        {
                                            checkNotDelete = true;
                                        }

                                        //choose save file path
                                        string fileNameAndPath = $"{owebHostEnvironment.WebRootPath}\\images\\teachers\\{file.FileName}";
                                        using (var stream = new FileStream(fileNameAndPath, FileMode.Create))
                                        {
                                            await file.CopyToAsync(stream);
                                            await stream.FlushAsync();
                                        }

                                        editStaff.ProfileImage = "/images/teachers/" + file.FileName;

                                        editStaff.Email = staff.Email;
                                        editStaff.Phone = staff.Phone;
                                        editStaff.Address = staff.Address;
                                        db.SaveChanges();

                                    if (checkNotDelete == false) //delete old file if false
                                    {
                                        System.GC.Collect();
                                        System.GC.WaitForPendingFinalizers();
                                        //check old painting exists
                                        if (System.IO.File.Exists(tempCurFilePath))
                                        {
                                            System.IO.File.Delete(tempCurFilePath);
                                        }
                                    }
                                        return RedirectToAction("Index", "Staffs");
                                }
                                else if (file.Length > 8388608)
                                {
                                    ViewBag.Painting = "Painting must be smaller than 8MB";
                                }
                                else
                                {
                                    ViewBag.Painting = "Painting must be .jpg or .png";
                                }
                            }//end check file null
                    }
                    else
                    {
                        ViewBag.Msg = "Invalid field Failed";
                    } //end check staff !null
                }//end check model valid
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
                    string tempCurFilePath = Path.Combine("wwwroot/", staff.ProfileImage.Substring(1)); //old painting

                    db.Staff.Remove(staff);
                    db.SaveChanges();

                    //check old painting exists to delete
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    if (System.IO.File.Exists(tempCurFilePath))
                    {
                        System.IO.File.Delete(tempCurFilePath);
                    }
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
