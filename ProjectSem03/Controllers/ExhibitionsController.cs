using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using SmartBreadcrumbs.Attributes;
using X.PagedList; //using for pagination

namespace ProjectSem03.Controllers
{
    public class ExhibitionsController : Controller
    {
        //connection to database
        ProjectDB db;
        public ExhibitionsController(ProjectDB db)
        {
            this.db = db;
        }

        [Breadcrumb("Exhibition List")]
        public IActionResult Index(string ename, string pname, int? page)
        {
            DateTime today = Convert.ToDateTime(DateTime.Today); //convert current day to comparable value
            var listUp = from e in db.Exhibition
                       join s in db.Staff on e.StaffId equals s.StaffId
                       where today <= e.StartDate.Date
                       select new CombineModels
                       {
                           Staffs = s,
                           Exhibitions = e
                       }; //get list of upcoming exhibitions
            ViewBag.data = listUp;

            //set number of records per page and starting page
            int maxsize = 3;
            int numpage = page ?? 1;
            var list = from e in db.Exhibition
                       join s in db.Staff
                       on e.StaffId equals s.StaffId
                       select new CombineModels
                       {
                           Staffs = s,
                           Exhibitions = e
                       }; //get combine list of exhibitions

            var model = list.ToList().ToPagedList(); //pagination
            if (ename != null) //if exhibition name is found
            {
                //show result of exhibition names
                var filter = list.Where(e => e.Exhibitions.ExhibitionName.ToLower().Contains(ename)).ToList().ToPagedList(numpage, maxsize);
                ViewBag.page = filter;
            }
            else if (pname != null) //if place is found
            {
                //show result of places
                var filter = list.Where(e => e.Exhibitions.Place.ToLower().Contains(pname)).ToList().ToPagedList(numpage, maxsize);
                ViewBag.page = filter;
            }
            else //empty
            {
                ViewBag.page = model;
            }
            return View();
        }

        [HttpGet]
        [Breadcrumb("Create Exhibition")]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("staffRole") == 2) //check session for Staff Role
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
        public IActionResult Create(Exhibition exhibition) //create new Exhibitions
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            try
            {
                //check conditions of creating
                if (ModelState.IsValid)
                {
                    //valid
                    bool checkOk = true;
                    //check duplicate ExhibitionName
                    var mName = db.Exhibition.SingleOrDefault(s => s.ExhibitionName.Equals(exhibition.ExhibitionName));
                    if (mName != null)
                    {
                        ViewBag.ExName = "Exhibition Name is already existed. Try again";
                        checkOk = false;
                    }

                    //check duplicate ExhibitionName
                    if (checkOk == false)
                    {
                        ViewBag.Msg = "Failed .......";
                        return View();
                    }
                    db.Exhibition.Add(exhibition);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Exhibitions");
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
        [Breadcrumb("Edit Exhibition")]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetInt32("staffRole") == 2)
            {
                var exh = db.Exhibition.Find(id); //Find exhibition Id

                var list = db.Staff.Where(s => s.Role.Equals(1) || s.Role.Equals(2));
                ViewBag.data = new SelectList(list, "StaffId", "StaffName", exh.StaffId);

                if (exh != null)
                {
                    return View(exh);
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
        public IActionResult Edit(Exhibition exhibition) //edit Exhibition
        {
            var list = db.Staff.Where(s => s.Role.Equals(1) || s.Role.Equals(2)); //check if Staff Role equals 1 or 2
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            try
            {
                var editExhibition = db.Exhibition.SingleOrDefault(e => e.ExhibitionId.Equals(exhibition.ExhibitionId)); //check Exhibtion Id

                if (ModelState.IsValid)
                {
                    if (editExhibition != null)
                    {
                        //valid
                        bool checkOk = true;
                        //check duplicate ExhibitionName
                        var mName = db.Exhibition.SingleOrDefault(s => s.ExhibitionName.Equals(exhibition.ExhibitionName) && s.ExhibitionName != editExhibition.ExhibitionName);
                        if (mName != null)
                        {
                            ViewBag.ExName = "Exhibition Name is already existed. Try again";
                            checkOk = false;
                        }

                        editExhibition.ExhibitionName = exhibition.ExhibitionName;
                        editExhibition.Place = exhibition.Place;
                        editExhibition.StartDate = exhibition.StartDate;
                        editExhibition.EndDate = exhibition.EndDate;
                        editExhibition.StaffId = exhibition.StaffId;

                        DateTime enddate = Convert.ToDateTime(exhibition.EndDate); //convert End Date to comparable value
                        DateTime startdate = Convert.ToDateTime(editExhibition.StartDate); //convert Start Date to comparable value
                        DateTime today = Convert.ToDateTime(DateTime.Today); //convert current End Date to comparable value
                        TimeSpan time = enddate - startdate; //calculate the time between End Date and Start Date
                        int edittime = time.Days; //convert into Day value

                        if (startdate.Date >= today) //check if start date is later than current date
                        {
                            if (edittime > 0) //check if end date is later than start date
                            {
                                //check duplicate ExhibitionName
                                if (checkOk==false)
                                {
                                    ViewBag.Msg = "Failed";
                                    return View();
                                }
                                db.SaveChanges();
                                return RedirectToAction("Index", "Exhibitions");
                            }
                            else
                            {
                                ViewBag.Msg = "EndDate cannot be earlier than StartDate. Update Failed";
                            }
                        }
                        else
                        {
                            ViewBag.Msg = "Exhibition has already started. Update Failed";
                        }
                    }
                    else
                    {
                        ViewBag.Msg = "Failed";
                    }
                }
                else
                {
                    ViewBag.Msg = "Failed";
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message;
            }
            return View();
        }

        public IActionResult Delete(int id) //Delete Exhibition
        {
            try
            {
                var exhibition = db.Exhibition.SingleOrDefault(e => e.ExhibitionId.Equals(id)); //check Exhibition Id
                if (exhibition != null)
                {
                    db.Exhibition.Remove(exhibition); //remove chosen Exhibition from database
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
