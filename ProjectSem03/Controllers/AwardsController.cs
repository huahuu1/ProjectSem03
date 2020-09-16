using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectSem03.Controllers
{
    public class AwardsController : Controller
    {
        ProjectDB db;
        public AwardsController(ProjectDB db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var list = from a in db.Award
                       join c in db.Competition on a.CompetitionID equals c.CompetitionId
                       join s in db.Staff on a.StaffId equals s.StaffId
                       join p in db.Posting on a.PostingID equals p.PostingId
                       select new CombineModels
                       {
                           Awards = a,
                           Competitions = c,
                           Staffs = s,
                           Postings = p
                       };
            return View(list);
        }

        public IActionResult Create()
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            var list2 = db.Competition.ToList();
            ViewBag.data2 = new SelectList(list2, "CompetitionId", "CompetitionName");

            var list3 = db.Posting.Where(p => p.Mark.Equals("best"));
            ViewBag.data3 = new SelectList(list3, "PostingId", "PostDescription");

            return View();
        }
        [HttpPost]
        public IActionResult Create(Award award)
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            var list2 = db.Competition.ToList();
            ViewBag.data2 = new SelectList(list2, "CompetitionId", "CompetitionName");

            var list3 = db.Posting.Where(p => p.Mark.Equals("best"));
            ViewBag.data3 = new SelectList(list3, "PostingId", "PostDescription");

            try
            {
                if (ModelState.IsValid)
                {
                    db.Award.Add(award);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Awards");
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
    }
}
