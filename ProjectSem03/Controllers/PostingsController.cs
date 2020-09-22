using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using SmartBreadcrumbs.Attributes;
using X.PagedList; //using for pagination

namespace ProjectSem03.Controllers
{
    public class PostingsController : Controller
    {
        //connection to database
        ProjectDB db;
        public PostingsController(ProjectDB db)
        {
            this.db = db;
        }

        [Breadcrumb("Posting List")]
        public IActionResult Index(string pname, int? page)
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

                //get combine list for Posting
                var list = from p in db.Posting
                           join d in db.Design on p.DesignID equals d.DesignId
                           join c in db.Competition on p.CompetitionId equals c.CompetitionId
                           join s in db.Staff on p.StaffId equals s.StaffId
                           select new CombineModels
                           {
                               Postings = p,
                               Designs = d,
                               Competitions = c,
                               Staffs = s
                           };
                var model = list.ToList().ToPagedList(numpage, maxsize); //pagination

                //check if result is found or not
                if (string.IsNullOrEmpty(pname))  //empty
                {
                    ViewBag.page = model;
                }
                else
                {
                    //show the result
                    var filter = list.Where(s => s.Postings.PostDescription.ToLower().Contains(pname)).ToList().ToPagedList();
                    ViewBag.page = filter;
                }
                return View();
            }
        }

        [HttpGet]
        [Breadcrumb("Edit Posting")]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetInt32("staffRole") == 2) //check session for Staff Role
            {
                var list = db.Staff.Where(s => s.Role.Equals(2)); //get list of staffs
                ViewBag.data = new SelectList(list, "StaffId", "StaffName");

                var posting = db.Posting.Find(id); //Find Posting id
                if (posting != null)
                {
                    return View(posting);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                //return to Index page of Staffs
                return RedirectToAction("Index", "Staffs");
            }
        }

        [HttpPost]
        public IActionResult Edit(Posting posting) //edit posting
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            try
            {
                var editPosting = db.Posting.SingleOrDefault(p => p.PostingId.Equals(posting.PostingId)); //check posting Id
                if (ModelState.IsValid)
                {
                    //check conditions of editting
                    if (editPosting != null)
                    {
                        //edit posting value
                        editPosting.PostDescription = posting.PostDescription;
                        editPosting.Mark = posting.Mark;
                        editPosting.Remark = posting.Remark;
                        editPosting.SoldStatus = posting.SoldStatus;
                        editPosting.PaidStatus = posting.PaidStatus;
                        editPosting.StaffId = posting.StaffId;
                        db.SaveChanges(); //save changes
                        return RedirectToAction("Index", "Postings"); //return to Index page of Positngs
                    }
                    else
                    {
                        ViewBag.Msg = "Failed ......."; //show error message if conditions are invalid
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message; //show other error messagess
            }
            return View();
        }
    }
}
