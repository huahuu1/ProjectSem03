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
            if (list.Count() == 0)
            {
                list = from a in db.Award
                                        join c in db.Competition on a.CompetitionID equals c.CompetitionId
                                        join s in db.Staff on a.StaffId equals s.StaffId
                                        select new CombineModels
                                        {
                                            Awards = a,
                                            Competitions = c,
                                            Staffs = s,
                                        };
                return View(list);
            }
            return View(list);
        }

        public IActionResult Create()
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            var list2 = db.Competition.ToList();
            ViewBag.data2 = new SelectList(list2, "CompetitionId", "CompetitionName");

            //var list3 = db.Posting.Where(p => p.Mark.Equals("best"));
            //ViewBag.data3 = new SelectList(list3, "PostingId", "PostDescription");

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

        public IActionResult Edit(int id)
        {
            var listAward = db.Award.Find(id);

            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName", listAward.StaffId);

            var list2 = db.Competition.ToList();
            ViewBag.data2 = new SelectList(list2, "CompetitionId", "CompetitionName", listAward.CompetitionID);

            var list3 = db.Posting.Where(p => p.Mark.Equals("best"));
            ViewBag.data3 = new SelectList(list3, "PostingId", "PostDescription", listAward.PostingID);

            if (listAward != null)
            {
                return View(listAward);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult Edit(Award award)
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            var list2 = db.Competition.ToList();
            ViewBag.data2 = new SelectList(list2, "CompetitionId", "CompetitionName");

            var list3 = db.Posting.Where(p => p.Mark.Equals("best"));
            ViewBag.data3 = new SelectList(list3, "PostingId", "PostDescription");

            try
            {
                var editAward = db.Award.SingleOrDefault(c => c.AwardId.Equals(award.AwardId));
                if (ModelState.IsValid)
                {
                    if (editAward != null)
                    {
                        editAward.AwardName = award.AwardName;
                        editAward.CompetitionID = award.CompetitionID;
                        editAward.StaffId = award.StaffId;
                        editAward.PostingID = award.PostingID;
                        db.SaveChanges();
                        return RedirectToAction("Index", "Awards");
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
                var award = db.Award.SingleOrDefault(s => s.AwardId.Equals(id));

                if (award != null)
                {
                    db.Award.Remove(award);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Awards");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Msg = ex.Message;
            }
            return View();
        }
    }
}
