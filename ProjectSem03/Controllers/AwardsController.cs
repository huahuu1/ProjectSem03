using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using SmartBreadcrumbs.Attributes;
using X.PagedList; //using for paginantion

namespace ProjectSem03.Controllers
{
    public class AwardsController : Controller
    {
        //connection to database
        ProjectDB db;
        public AwardsController(ProjectDB db)
        {
            this.db = db;
        }

        [Breadcrumb("Award List")]
        public IActionResult Index(string aname, int? page)
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

                //get combined list for Award
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
                var model = list.ToList().ToPagedList(numpage, maxsize); //pagination

                //check if the search has result or not
                if (string.IsNullOrEmpty(aname)) //empty
                {
                    ViewBag.page = model;
                }
                else
                {
                    //show the search result
                    var filter = list.Where(s => s.Awards.AwardName.ToLower().Contains(aname)).ToList().ToPagedList();
                    ViewBag.page = filter;
                }
                return View();
            }
        }

        [HttpGet]
        [Breadcrumb("Create Award")]
        public IActionResult Create()
        {
            //Get list of Awards
            var listAward = db.Award.ToList();
            
            //check if current Staff Role equals 2 or not
            if(HttpContext.Session.GetInt32("staffRole") == 2)
            {
                //show list of staffs
                var list = db.Staff.Where(s => s.Role.Equals(2));
                ViewBag.data = new SelectList(list, "StaffId", "StaffName");

                // show competition is not with any awards
                var list2 = db.Competition.Where(c=> !db.Award.Select(a=>a.CompetitionID).Contains(c.CompetitionId));
                ViewBag.data2 = new SelectList(list2, "CompetitionId", "CompetitionName");

                //show list of "best" postings
                var list3 = db.Posting.Where(p => p.Mark.Equals("best"));
                ViewBag.data3 = new SelectList(list3, "PostingId", "PostDescription");

                return View();
            }
            else
            {
                //return to Index page of Awards Controller
                return RedirectToAction("Index", "Awards");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Award award)
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            var list2 = db.Competition.Where(c => !db.Award.Select(a => a.CompetitionID).Contains(c.CompetitionId));
            ViewBag.data2 = new SelectList(list2, "CompetitionId", "CompetitionName");

            var list3 = db.Posting.Where(p => p.Mark.Equals("best"));
            ViewBag.data3 = new SelectList(list3, "PostingId", "PostDescription");

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
                    db.Award.Add(award);//Add new Award
                    db.SaveChanges();//Save Changes
                    return RedirectToAction("Index", "Awards");// return to Index page of Awards Controller
                }
                else
                {
                    ViewBag.Msg = "Failed .......";
                }
            }
            catch (Exception e)
            {
                //Show error messages
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
                return RedirectToAction("Index", "Awards");
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

            //start
            try
            {
                //check Award ID
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
                //Find Award Id
                var award = db.Award.SingleOrDefault(s => s.AwardId.Equals(id));

                if (award != null)
                {
                    db.Award.Remove(award); //remove chosen Award Id from database
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
