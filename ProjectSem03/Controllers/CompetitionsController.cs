using System;
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
                int maxsize = 3;
                int numpage = page ?? 1;

                //get combine list for Competition
                var list = from c in db.Competition
                           join s in db.Staff on c.StaffId equals s.StaffId
                           select new CombineModels
                           {
                               Staffs = s,
                               Competitions = c
                           };
                var model = list.ToList().ToPagedList(); //pagination

                //check if result is found or not
                if (string.IsNullOrEmpty(cname)) //empty
                {
                    ViewBag.page = model;
                }
                else
                {
                    //show the result
                    var filter = list.Where(c=>c.Competitions.CompetitionName.ToLower().Contains(cname)).ToList().ToPagedList(numpage, maxsize);
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
                var list = db.Staff.Where(s => s.Role.Equals(2)); //get list of staffs
                ViewBag.data = new SelectList(list, "StaffId", "StaffName");
                return View();
            }
            else
            {
                //return to Index page of Staffs
                return RedirectToAction("Index", "Staffs");
            }
        }
        [HttpPost]
        public IActionResult Create(Competition competition, IFormFile file) //create new comeptition
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            try
            {
                if (ModelState.IsValid)
                {
                    //check conditions of creating
                    if (file.Length > 0)
                    {
                        string path = Path.Combine("wwwroot/images", file.FileName); //destination of image source
                        var stream = new FileStream(path, FileMode.Create); //create file stream
                        file.CopyToAsync(stream); //copy image file
                        competition.CompetitionImages = "/images/" + file.FileName; //set copied image to CompetitionImage

                        db.Competition.Add(competition); //add new competition
                        db.SaveChanges(); //save changes
                        return RedirectToAction("Index", "Competitions"); ////return to Index page of Competitions
                    }
                }
                else
                {
                    ViewBag.Msg = "Failed ......."; //show error message if conditions are invalid
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
                return RedirectToAction("Index", "Staffs");
            }
        }

        [HttpPost]
        public IActionResult Edit(Competition competition, IFormFile file) //edit competition
        {
            try
            {
                var editCompetition = db.Competition.SingleOrDefault(c => c.CompetitionId.Equals(competition.CompetitionId)); //check CompetitionId
                if (ModelState.IsValid)
                {
                    if (editCompetition != null)
                    {
                        if (file == null)
                        {
                            //edit Competition values without choosing image
                            editCompetition.CompetitionName = competition.CompetitionName;
                            editCompetition.StartDate = competition.StartDate;
                            editCompetition.EndDate = competition.EndDate;
                            editCompetition.Description = competition.Description;
                            editCompetition.StaffId = competition.StaffId;
                            db.SaveChanges();
                            return RedirectToAction("Index", "Competitions");
                        }
                        else if (file != null && file.Length > 0)
                        {
                            //edit Competition values with choosing image
                            string path = Path.Combine("wwwroot/images", file.FileName);
                            var stream = new FileStream(path, FileMode.Create);
                            file.CopyToAsync(stream);
                            competition.CompetitionImages = "/images/" + file.FileName;
                            editCompetition.CompetitionName = competition.CompetitionName;
                            editCompetition.StartDate = competition.StartDate;
                            editCompetition.EndDate = competition.EndDate;
                            editCompetition.Description = competition.Description;
                            editCompetition.CompetitionImages = competition.CompetitionImages;
                            editCompetition.StaffId = competition.StaffId;
                            db.SaveChanges();
                            return RedirectToAction("Index", "Competitions");
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
                    db.Competition.Remove(competition); //remove chosen Competition from database
                    db.SaveChanges();
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
