using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectSem03.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectSem03.Controllers
{
    public class ExhibitionsController : Controller
    {
        ProjectDB db;
        public ExhibitionsController(ProjectDB db)
        {
            this.db = db;
        }

        public IActionResult Index(string ename, string pname)
        {
            var list = from e in db.Exhibition
                       join s in db.Staff
                       on e.StaffId equals s.StaffId
                       select new CombineModels
                       {
                           Staffs = s,
                           Exhibitions = e
                       };
            if (ename != null)
            {
                var filter = list.Where(e => e.Exhibitions.ExhibitionName.ToLower().Contains(ename) || e.Exhibitions.ExhibitionName.ToUpper().Contains(ename));
                return View(filter);
            }
            else if (pname != null)
            {
                var filter = list.Where(e => e.Exhibitions.Place.ToLower().Contains(pname) || e.Exhibitions.Place.ToUpper().Contains(pname));
                return View(filter);
            }
            else
            {
                return View(list);
            }
        }
        
        public IActionResult UpcomingIndex()
        {
            DateTime today = Convert.ToDateTime(DateTime.Today);
            var list = from e in db.Exhibition
                       join s in db.Staff on e.StaffId equals s.StaffId
                       where today <= e.StartDate.Date
                       select new CombineModels
                       {
                           Staffs = s,
                           Exhibitions = e
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
        public IActionResult Create(Exhibition exhibition)
        {
            var list = db.Staff.Where(s => s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            try
            {
                if (ModelState.IsValid)
                {
                    db.Exhibition.Add(exhibition);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Exhibitions");
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
            var exh = db.Exhibition.Find(id);

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
        [HttpPost]
        public IActionResult Edit(Exhibition exhibition)
        {
            var list = db.Staff.Where(s => s.Role.Equals(1) || s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            try
            {
                var editExhibition = db.Exhibition.SingleOrDefault(e => e.ExhibitionId.Equals(exhibition.ExhibitionId));

                if (ModelState.IsValid)
                {
                    if (editExhibition != null)
                    {
                        editExhibition.ExhibitionName = exhibition.ExhibitionName;
                        editExhibition.Place = exhibition.Place;
                        editExhibition.StartDate = exhibition.StartDate;
                        editExhibition.EndDate = exhibition.EndDate;
                        editExhibition.StaffId = exhibition.StaffId;

                        DateTime enddate = Convert.ToDateTime(exhibition.EndDate);
                        DateTime startdate = Convert.ToDateTime(editExhibition.StartDate);
                        DateTime today = Convert.ToDateTime(DateTime.Today);
                        TimeSpan time = enddate - startdate;
                        int edittime = time.Days;

                        if (startdate.Date >= today)
                        {
                            if (edittime > 0)
                            {
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

        public IActionResult EditUpcoming(int id)
        {
            var list = db.Staff.Where(s => s.Role.Equals(1) || s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            var exh = db.Exhibition.Find(id);
            if (exh != null)
            {
                return View(exh);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult EditUpcoming(Exhibition exhibition)
        {
            var list = db.Staff.Where(s => s.Role.Equals(1) || s.Role.Equals(2));
            ViewBag.data = new SelectList(list, "StaffId", "StaffName");

            try
            {
                var editExhibition = db.Exhibition.SingleOrDefault(e => e.ExhibitionId.Equals(exhibition.ExhibitionId));

                if (ModelState.IsValid)
                {
                    if (editExhibition != null)
                    {
                        editExhibition.ExhibitionName = exhibition.ExhibitionName;
                        editExhibition.Place = exhibition.Place;
                        editExhibition.StartDate = exhibition.StartDate;
                        editExhibition.EndDate = exhibition.EndDate;
                        editExhibition.StaffId = exhibition.StaffId;

                        DateTime enddate = Convert.ToDateTime(exhibition.EndDate);
                        DateTime startdate = Convert.ToDateTime(editExhibition.StartDate);
                        DateTime today = Convert.ToDateTime(DateTime.Today);
                        TimeSpan time = enddate - startdate;
                        int edittime = time.Days;

                        if (exhibition.StartDate.Date >= today)
                        {
                            if (edittime > 0)
                            {
                                db.SaveChanges();
                                return RedirectToAction("UpcomingIndex", "Exhibitions");
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

        public IActionResult Delete(int id)
        {
            try
            {
                var exhibition = db.Exhibition.SingleOrDefault(e => e.ExhibitionId.Equals(id));
                if (exhibition != null)
                {
                    db.Exhibition.Remove(exhibition);
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
