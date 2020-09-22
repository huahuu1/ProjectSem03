using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using SmartBreadcrumbs.Attributes;

namespace ProjectSem03.Controllers
{
    public class AwardsController : Controller
    {
        ProjectDB db;
        public AwardsController(ProjectDB db)
        {
            this.db = db;
        }

        [Breadcrumb("Award List")]
        public IActionResult Index(string aname)
        {
            if (HttpContext.Session.GetString("staffId") == null) //check session
            {
                return RedirectToAction("Login");
            }
            else
            {
                var list = from a in db.Award
                           join c in db.Competition on a.CompetitionID equals c.CompetitionId
                           join s in db.Staff on a.StaffId equals s.StaffId
                           join p in db.Posting on a.PostingID equals p.PostingId into groupjoin
                           from p in groupjoin.DefaultIfEmpty()
                           select new CombineModels
                           {
                               Awards = a,
                               Competitions = c,
                               Staffs = s,
                               Postings = p
                           };
                if (string.IsNullOrEmpty(aname))
                {
                    return View(list);
                }
                else
                {
                    var filter = list.Where(s => s.Awards.AwardName.ToLower().Contains(aname) || s.Awards.AwardName.ToUpper().Contains(aname));
                    return View(filter);
                }
            }
        }

        [HttpGet]
        [Breadcrumb("Create Award")]
        public IActionResult Create()
        {
            var listAward = db.Award.ToList();
            
            if(HttpContext.Session.GetInt32("staffRole") == 2)
            {
                var list = db.Staff.Where(s => s.Role.Equals(2));
                ViewBag.data = new SelectList(list, "StaffId", "StaffName");

                // show competition is not with any awards
                var list2 = db.Competition.Where(c=> !db.Award.Select(a=>a.CompetitionID).Contains(c.CompetitionId));
                ViewBag.data2 = new SelectList(list2, "CompetitionId", "CompetitionName");

                var list3 = db.Posting.Where(p => p.Mark.Equals("best"));
                ViewBag.data3 = new SelectList(list3, "PostingId", "PostDescription");

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Staffs");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Award award)
        {
            //call from get
            if (HttpContext.Session.GetInt32("staffRole") == 2)
            {
                var list = db.Staff.Where(s => s.Role.Equals(2));
                ViewBag.data = new SelectList(list, "StaffId", "StaffName");

                // show competition is not with any awards
                var list2 = db.Competition.Where(c => !db.Award.Select(a => a.CompetitionID).Contains(c.CompetitionId));
                ViewBag.data2 = new SelectList(list2, "CompetitionId", "CompetitionName");

                var list3 = db.Posting.Where(p => p.Mark.Equals("best"));
                ViewBag.data3 = new SelectList(list3, "PostingId", "PostDescription");
            }

            //start
            try
            {
                if (ModelState.IsValid)
                {
                    //valid
                    bool checkOk = true;
                    //check picture duplicate AwardName
                    var mName = db.Award.SingleOrDefault(s => s.AwardName.Equals(award.AwardName));
                    if (mName != null)
                    {
                        ViewBag.AwName = "Award Name is already existed. Try again";
                        checkOk = false;
                    }
                    //check duplicate AwardName
                    if (checkOk == false)
                    {
                        ViewBag.Msg = "Failed";
                        return View();
                    }
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

        [HttpGet]
        [Breadcrumb("Edit Award")]
        public IActionResult Edit(int id)
        {
            if(HttpContext.Session.GetInt32("staffRole") == 2)
            {
                var listAward = db.Award.Find(id);
                HttpContext.Session.SetInt32("Awardid", id);
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
            else
            {
                return RedirectToAction("Index", "Staffs");
            }
        }
        [HttpPost]
        public IActionResult Edit(Award award)
        {

            //call from get
            var listAward = db.Award.Find(HttpContext.Session.GetInt32("Awardid"));

            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName", listAward.StaffId);

            var list2 = db.Competition.ToList();
            ViewBag.data2 = new SelectList(list2, "CompetitionId", "CompetitionName", listAward.CompetitionID);

            var list3 = db.Posting.Where(p => p.Mark.Equals("best"));
            ViewBag.data3 = new SelectList(list3, "PostingId", "PostDescription", listAward.PostingID);

            //start
            try
            {
                var editAward = db.Award.SingleOrDefault(c => c.AwardId.Equals(award.AwardId));
                if (ModelState.IsValid)
                {
                    if (editAward != null)
                    {
                        //valid
                        bool checkOk = true;
                        //check picture duplicate AwardName
                        var mName = db.Award.SingleOrDefault(s => s.AwardName.Equals(award.AwardName) && s.AwardName != editAward.AwardName);
                        if (mName != null)
                        {
                            ViewBag.AwName = "Award Name is already existed. Try again";
                            checkOk = false;
                        }
                        //check duplicate AwardName
                        if (checkOk == false)
                        {
                            ViewBag.Msg = "Failed";
                            return View();                            
                        }
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
