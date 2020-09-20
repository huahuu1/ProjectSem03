using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectSem03.Models;

namespace ProjectSem03.Controllers
{
    public class HomeController : Controller
    {
        ProjectDB db;
        public HomeController(ProjectDB db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            DateTime localDate = DateTime.Now;
            //ViewBag.data = db.Competition.ToList();
            ViewBag.data = db.Competition.Where(c => c.StartDate <= localDate && c.EndDate >= localDate);

            var list = from p in db.Posting
                       join s in db.Staff on p.StaffId equals s.StaffId
                       join c in db.Competition on p.CompetitionId equals c.CompetitionId
                       join d in db.Design on p.DesignID equals d.DesignId
                       join stu in db.Student on d.StudentId equals stu.StudentId
                       where p.Mark.Equals("best")
                       select new CombineModels
                       {
                           Postings = p,
                           Designs = d,
                           Competitions = c,
                           Staffs = s,
                           Students = stu
                       };
            return View(list);
        }

        public IActionResult Exhibition(string ename)
        {
            
            ViewBag.Exhibition = db.Exhibition.ToList();

            var exh = db.Exhibition.ToList();
            ViewBag.data = new SelectList(exh, "ExhibitionId", "ExhibitionName");

            var list = from e in db.Exhibition
                       join d in db.Design on e.ExhibitionId equals d.ExhibitionID
                       join stu in db.Student on d.StudentId equals stu.StudentId
                       where e.ExhibitionId.Equals(d.ExhibitionID)
                       orderby e.ExhibitionId
                       select new CombineModels
                       {
                           Exhibitions = e,
                           Designs = d,
                           Students = stu
                       };

            if (string.IsNullOrEmpty(ename))
            {
                return View(list);
            }
            else
            {
                int eId = int.Parse(ename);
                var filter = list.Where(d => d.Exhibitions.ExhibitionId.Equals(eId));
                return View(filter);
            }
        }

        private List<CombineModels> designStudentList()
        {
            var list = (from d in db.Design
                        join s in db.Student on d.StudentId equals s.StudentId
                        where s.StudentId.Equals(HttpContext.Session.GetString("studentid")) //check student
                        select new CombineModels
                        {
                            Designs = d,
                            Students = s
                        }).ToList();
            return list;
        }

        public IActionResult Upload(int id)
        {
            if (HttpContext.Session.GetString("ename") == null) //check session
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Viewbag list of student designs
                ViewBag.designList = designStudentList();
                return View();
            }
        }

        //[HttpGet]
        //public IActionResult Login()
        //{
        //    return View();
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string accName, string accPass)
        {
            try
            {
                var staff = db.Staff.SingleOrDefault(s => s.Email.Equals(accName));
                var student = db.Student.SingleOrDefault(s => s.Email.Equals(accName));

                if (staff != null)
                {
                    var key = "b14ca5898a4e4133bbce2ea2315a1916";
                    staff.Password = AesEncDesc.DecryptString(key, staff.Password);
                    if (staff.Password.Equals(accPass))
                    {
                        HttpContext.Session.SetString("ename", accName);
                        HttpContext.Session.SetString("staffName", staff.StaffName);
                        HttpContext.Session.SetString("staffId", staff.StaffId);
                        HttpContext.Session.SetString("staffImage", staff.ProfileImage);
                        HttpContext.Session.SetInt32("staffRole", staff.Role);
                        return RedirectToAction("Index", "Admin", new { area = "" });
                    }
                    else
                    {
                        ViewBag.Msg = "Invalid Pasword....";
                    }
                }
                else if (student != null)
                {
                    var key = "b14ca5898a4e4133bbce2ea2315a1916";
                    student.Password = AesEncDesc.DecryptString(key, student.Password);
                    if (student.Password.Equals(accPass))
                    {
                        HttpContext.Session.SetString("ename", student.FirstName + " " + student.LastName);
                        HttpContext.Session.SetString("studentid", student.StudentId);
                        HttpContext.Session.SetString("studentImage", student.ProfileImage);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Msg = "Invalid Pasword....";
                    }
                }
                else
                {
                    ViewBag.Msg = "Invalid Username....";
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message;
            }
            return View();
        }

        public IActionResult Logout()
        {
            var model = HttpContext.Session.GetString("ename");
            if (model != null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");

            }
            return RedirectToAction("Index", "Home");
        }
    }
}
