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
        [RequestFormLimits(MultipartBodyLengthLimit = 8388608)]
        [RequestSizeLimit(8388608)]
        public async Task<IActionResult> Create(Staff staff, IFormFile file, [FromServices] IWebHostEnvironment owebHostEnvironment)
        {
            var staffid = db.Staff.SingleOrDefault(s => s.StaffId.Equals(staff.StaffId));
            var email = db.Staff.SingleOrDefault(e => e.Email.Equals(staff.Email));
            var phone = db.Staff.SingleOrDefault(p => p.Phone.Equals(staff.Phone));

            try
            {
                if (ModelState.IsValid)
                {

                    //valid
                    bool checkOk = true;
                    //check unique email phone
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

                        string ext = Path.GetExtension(file.FileName);
                        var today = DateTime.Now;
                        if ((file.Length > 0 && file.Length < 8388608) && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".png")))
                        {

                            //check picture duplicate, unique email or phone
                            if (checkOk == false)
                            {
                                return View();
                            }

                            //choose image
                            string fileNameAndPath = $"{owebHostEnvironment.WebRootPath}\\images\\teachers\\{file.FileName}";
                            using (var stream = new FileStream(fileNameAndPath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                                await stream.FlushAsync();
                            }
                            staff.ProfileImage = "/images/teachers/" + file.FileName;

                            var key = "b14ca5898a4e4133bbce2ea2315a1916";
                            staff.Password = AesEncDesc.EncryptString(key, staff.Password);

                            db.Staff.Add(staff);
                            db.SaveChanges();
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
                    }
                    else
                    {
                        ViewBag.Image = "Image Upload Container Cannot Be Empty";
                    }
                }
                else
                {
                    ViewBag.Msg = "Failed .......";
                }//end check file lenght
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
        [RequestFormLimits(MultipartBodyLengthLimit = 8388608)]
        [RequestSizeLimit(8388608)]
        public async Task<IActionResult> Edit(Staff staff, IFormFile file, [FromServices] IWebHostEnvironment owebHostEnvironment)
        {
            try
            {
                var editStaff = db.Staff.SingleOrDefault(c => c.StaffId.Equals(staff.StaffId));
                if (ModelState.IsValid)
                {
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
                                        ViewBag.Painting = "File name already exists";
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


                                        bool checkNotDelete = false;
                                        string tempCurFilePath = Path.Combine("wwwroot/", editStaff.ProfileImage.Substring(1)); //old painting                                        
                                        if (("/images/" + file.FileName).Equals(editStaff.ProfileImage))
                                        {
                                            checkNotDelete = true;
                                        }
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

                                    if (checkNotDelete == false)
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

        public IActionResult Delete(string id)
        {
            try
            {
                var staff = db.Staff.SingleOrDefault(s => s.StaffId.Equals(id));

                if (staff != null)
                {
                    string tempCurFilePath = Path.Combine("wwwroot/", staff.ProfileImage.Substring(1)); //old painting

                    db.Staff.Remove(staff);
                    db.SaveChanges();

                    //check old painting exists
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
                return BadRequest("Cannot delete object that is existed in other tables");
            }
            return View();
        }
    }
}
