﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;
using SmartBreadcrumbs.Attributes;
using X.PagedList; //using for pagination
using Microsoft.AspNetCore.Hosting;

namespace ProjectSem03.Controllers
{
    public class CompetitionsController : Controller
    {
        //connection to database
        ProjectDB db;
        public CompetitionsController(ProjectDB db)
        {
            this.db = db;
        }

        [Breadcrumb("Competition List")]
        public IActionResult Index(string cname, int? page)
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

                //get combine list for Competition
                var list = from c in db.Competition
                           join s in db.Staff on c.StaffId equals s.StaffId
                           select new CombineModels
                           {
                               Staffs = s,
                               Competitions = c
                           };
                var model = list.ToList().ToPagedList(numpage, maxsize); //pagination

                //check if result is found or not
                if (string.IsNullOrEmpty(cname)) //empty
                {
                    ViewBag.page = model;
                }
                else
                {
                    //show the result
                    var filter = list.Where(c=>c.Competitions.CompetitionName.ToLower().Contains(cname)).ToList().ToPagedList();
                    ViewBag.page = filter;
                }
                return View();
            }
        }

        [HttpGet]
        [Breadcrumb("Create Competition")]
        public IActionResult Create()
        {
            if(HttpContext.Session.GetInt32("staffRole") == 2) //check session for Staff Role
            {
                
                
                var list = db.Staff.Where(s => s.Role.Equals(2));
                ViewBag.data = new SelectList(list, "StaffId", "StaffName");
                return View();
                
            }
            else
            {
                //return to Index page of Competitions
                return RedirectToAction("Index", "Competitions");
            }
        }
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 8388608)]
        [RequestSizeLimit(8388608)]
        public async Task<IActionResult> Create(Competition competition, IFormFile file, [FromServices] IWebHostEnvironment owebHostEnvironment)
        {

            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            //start here
            try
            {
                if (ModelState.IsValid)
                {
                    //valid
                    bool checkOk = false;
                    //check duplicate CompetitionName
                    var mName = db.Competition.SingleOrDefault(c => c.CompetitionName.Equals(competition.CompetitionName));
                    if (mName != null)
                    {
                        ViewBag.Cpt = "Competition Name is already existed. Try again";
                        checkOk = false;
                    }
                    //check duplicate Competition Time
                    var timeStart = db.Competition.Where(c => c.EndDate.Date >= competition.StartDate.Date);

                    if (timeStart.Count() > 0)
                    {
                        ViewBag.TimeStart = "Date is already existed. Try again";
                        checkOk = false;
                    }
                    else if (timeStart.Count() <= 0 && competition.StartDate.Date >= competition.EndDate.Date)
                    {
                        ViewBag.TimeEnd = "End Date must be larger than Start Date. Try again";
                        checkOk = false;
                    }
                    //check endDate must be larger than starDate
                    else if (timeStart.Count() > 0 && competition.StartDate.Date >= competition.EndDate.Date)
                    {
                        ViewBag.TimeStart = "Date is already existed. Try again";
                        ViewBag.TimeEnd = "End Date must be larger than Start Date. Try again";
                        checkOk = false;
                    }
                    
                    else if (timeStart.Count() <= 0 && competition.StartDate.Date < competition.EndDate.Date)
                    {
                        checkOk = true;
                    }

                    if (checkOk == true)
                    {
                        if (file != null)
                        {
                            //check Images duplicate
                            var modelDuplicate = db.Competition.SingleOrDefault(c => c.CompetitionImages.Equals("/images/" + file.FileName));
                            //if (modelDuplicate != null)
                            //{
                            if (modelDuplicate != null)
                            {
                                ViewBag.CImage = "File name already exists";
                                checkOk = false;
                            }

                            //check file
                            string ext = Path.GetExtension(file.FileName);
                            if ((file.Length > 0 && file.Length < 8388608) && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".png"))) //painting must be .jpg or .png
                            {
                                //check duplicate Images, CompetitionName
                                if (checkOk == false)
                                {
                                    ViewBag.Msg = "Fail";
                                    return View();
                                }

                                //choose images
                                string fileNameAndPath = $"{owebHostEnvironment.WebRootPath}\\images\\{file.FileName}";
                                using (var stream = new FileStream(fileNameAndPath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                    await stream.FlushAsync();
                                }

                                competition.CompetitionImages = "/images/" + file.FileName;

                                db.Competition.Add(competition);
                                db.SaveChanges();
                                return RedirectToAction("Index", "Competitions");
                            }
                            else if (file.Length > 8388608)
                            {
                                ViewBag.CImage = "CompetitionImages must be smaller than 8MB";
                            }
                            else
                            {
                                ViewBag.CImage = "CompetitionImages must be .jpg or .png";
                            }
                        }
                        else
                        {
                            ViewBag.CImage = "CompetitionImages Required";
                        }
                    }
                }
                else
                {
                    ViewBag.Msg = "Failed .......";
                }
            }
            catch (Exception e)
            {
                ViewBag.msg = e.Message; //show other error messages
            }
            return View();
        }

        [HttpGet]
        [Breadcrumb("Edit Competition")]
        public IActionResult Edit(int id)
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            if (HttpContext.Session.GetInt32("staffRole") == 2)
            {
                var competition = db.Competition.Find(id); //find CompetitionId
                if (competition != null)
                {
                    return View(competition);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "Competitions");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 8388608)]
        [RequestSizeLimit(8388608)]
        public async Task<IActionResult> Edit(Competition competition, IFormFile file, [FromServices] IWebHostEnvironment owebHostEnvironment)
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");
            try
            {
                var editCompetition = db.Competition.SingleOrDefault(c => c.CompetitionId.Equals(competition.CompetitionId)); //check CompetitionId
                if (ModelState.IsValid)
                {
                    if (editCompetition != null)
                    {
                        //valid
                        bool checkOk = false;
                        //check duplicate CompetitionName
                        var mName = db.Competition.SingleOrDefault(c => c.CompetitionName.Equals(competition.CompetitionName) && c.CompetitionName != editCompetition.CompetitionName);
                        if (mName != null)
                        {
                            ViewBag.Cpt = "Competition Name is already existed. Try again";
                            checkOk = false;
                        }

                        //check duplicate Competition Time
                        var timeStart = db.Competition.Where(c => c.EndDate.Date >= competition.StartDate.Date);

                        if (timeStart.Count() > 0)
                        {
                            ViewBag.TimeStart = "Date is already existed. Try again";
                            checkOk = false;
                        }
                        else if (timeStart.Count() <= 0 && competition.StartDate.Date >= competition.EndDate.Date)
                        {
                            ViewBag.TimeEnd = "End Date must be larger than Start Date. Try again";
                            checkOk = false;
                        }
                        //check endDate must be larger than starDate
                        else if (timeStart.Count() > 0 && competition.StartDate.Date >= competition.EndDate.Date)
                        {
                            ViewBag.TimeStart = "Date is already existed. Try again";
                            ViewBag.TimeEnd = "End Date must be larger than Start Date. Try again";
                            checkOk = false;
                        }

                        else if (timeStart.Count() <= 0 && competition.StartDate.Date < competition.EndDate.Date)
                        {
                            checkOk = true;
                        }
                        if (checkOk == true)
                        {
                            if (file == null)
                            {
                                //check duplicate CompetitionName
                                if (checkOk == false)
                                {
                                    ViewBag.Msg = "Fail";
                                    return View();
                                }
                                editCompetition.CompetitionName = competition.CompetitionName;
                                editCompetition.StartDate = competition.StartDate;
                                editCompetition.EndDate = competition.EndDate;
                                editCompetition.Description = competition.Description;
                                editCompetition.StaffId = competition.StaffId;
                                db.SaveChanges();
                                return RedirectToAction("Index", "Competitions");
                            }
                            else
                            {

                                //check Images duplicate
                                var modelDuplicate = db.Competition.SingleOrDefault(c => c.CompetitionImages.Equals("/images/" + file.FileName) && c.CompetitionImages != editCompetition.CompetitionImages);
                                //if (modelDuplicate != null)
                                //{
                                if (modelDuplicate != null)
                                {
                                    ViewBag.CImage = "File name already exists";
                                    checkOk = false;
                                }
                                //check file
                                string ext = Path.GetExtension(file.FileName);
                                var today = DateTime.Now;
                                if ((file.Length > 0 && file.Length < 8388608) && (ext.ToLower().Equals(".jpg") || ext.ToLower().Equals(".png"))) //painting must be .jpg or .png
                                {
                                    //check duplicate Images, CompetitionName
                                    if (checkOk == false)
                                    {
                                        ViewBag.Msg = "Fail";
                                        return View();
                                    }

                                    //choose images
                                    bool checkNotDelete = false;
                                    string tempCurFilePath = Path.Combine("wwwroot/", editCompetition.CompetitionImages.Substring(1)); //old painting
                                    if (("/images/" + file.FileName).Equals(editCompetition.CompetitionImages))
                                    {
                                        checkNotDelete = true;
                                    }
                                    string fileNameAndPath = $"{owebHostEnvironment.WebRootPath}\\images\\{file.FileName}";
                                    using (var stream = new FileStream(fileNameAndPath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                        await stream.FlushAsync();
                                    }

                                    competition.CompetitionImages = "/images/" + file.FileName;
                                    editCompetition.CompetitionName = competition.CompetitionName;
                                    editCompetition.StartDate = competition.StartDate;
                                    editCompetition.EndDate = competition.EndDate;
                                    editCompetition.Description = competition.Description;
                                    editCompetition.CompetitionImages = competition.CompetitionImages;
                                    editCompetition.StaffId = competition.StaffId;
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
                                    return RedirectToAction("Index", "Competitions");
                                }
                                else if (file.Length > 8388608)
                                {
                                    ViewBag.CImage = "CompetitionImages must be smaller than 8MB";
                                }
                                else
                                {
                                    ViewBag.CImage = "CompetitionImages must be .jpg or .png";
                                }
                            }//end check file null
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

        public IActionResult Delete(int id) //delete competition
        {
            try
            {
                var competition = db.Competition.SingleOrDefault(c => c.CompetitionId.Equals(id)); //find Competition Id
                if (competition != null)
                {
                    string tempCurFilePath = Path.Combine("wwwroot/", competition.CompetitionImages.Substring(1)); //old painting

                    db.Competition.Remove(competition);
                    db.SaveChanges();

                    //check old painting exists
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    if (System.IO.File.Exists(tempCurFilePath))
                    {
                        System.IO.File.Delete(tempCurFilePath);
                    }
                    return RedirectToAction("Index", "Competitions");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
