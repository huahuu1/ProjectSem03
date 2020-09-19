using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ProjectSem03.Controllers
{
    public class CompetitionsController : Controller
    {
        ProjectDB db;
        public CompetitionsController(ProjectDB db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var list = from c in db.Competition
                       join s in db.Staff
                       on c.StaffId equals s.StaffId
                       select new CombineModels
                       {
                           Staffs = s,
                           Competitions = c
                       };
            return View(list);
        }

        public IActionResult Create()
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Competition competition, IFormFile file)
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            try
            {
                if (ModelState.IsValid)
                {
                    if (file.Length > 0)
                    {
                        string path = Path.Combine("wwwroot/images", file.FileName);
                        var stream = new FileStream(path, FileMode.Create);
                        file.CopyToAsync(stream);
                        competition.CompetitionImages = "/images/" + file.FileName;

                        db.Competition.Add(competition);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Competitions");
                    }
                }
                else
                {
                    ViewBag.Msg = "Failed .......";
                }
            }
            catch (Exception e)
            {
                ViewBag.msg = e.Message;
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            var competition = db.Competition.Find(id);
            if (competition != null)
            {
                return View(competition);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Edit(Competition competition, IFormFile file)
        {
            try
            {
                var editCompetition = db.Competition.SingleOrDefault(c => c.CompetitionId.Equals(competition.CompetitionId));
                if (ModelState.IsValid)
                {
                    if (editCompetition != null)
                    {
                        if (file == null)
                        {
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

        public IActionResult Delete(int id)
        {
            try
            {
                var competition = db.Competition.SingleOrDefault(c => c.CompetitionId.Equals(id));
                if (competition != null)
                {
                    db.Competition.Remove(competition);
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
