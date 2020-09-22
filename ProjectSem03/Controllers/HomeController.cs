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
        //connection to database
        ProjectDB db;
        public HomeController(ProjectDB db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            DateTime localDate = DateTime.Now; //convert current day to comparable value
            //ViewBag.data = db.Competition.ToList();
            ViewBag.data = db.Competition.Where(c => c.StartDate <= localDate && c.EndDate >= localDate); //get list of upcoming competitions

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
                       }; //get combine list of "best" designs
            return View(list);
        }

        public IActionResult Exhibition(string stuname) //Index of Exhibition Page
        {
            ViewBag.Exhibition = db.Exhibition.ToList(); //Viewbag list of exhibitions
            
            //var exh = db.Exhibition.ToList();
            //ViewBag.data = new SelectList(exh, "ExhibitionId", "ExhibitionName");

            var list = from e in db.Exhibition
                       join d in db.Design on e.ExhibitionId equals d.ExhibitionID
                       join stu in db.Student on d.StudentId equals stu.StudentId
                       where e.ExhibitionId.Equals(d.ExhibitionID)
                       orderby e.StartDate
                       select new CombineModels
                       {
                           Exhibitions = e,
                           Designs = d,
                           Students = stu
                       };//get combine list of exhibitions

            //check if result is found or not
            if (string.IsNullOrEmpty(stuname)) //empty
            {
                return View(list);
            }
            else
            {
                //show the result
                var filter = from e in db.Exhibition
                       join d in db.Design on e.ExhibitionId equals d.ExhibitionID
                       join stu in db.Student on d.StudentId equals stu.StudentId
                       where stu.FirstName.Contains(stuname) || stu.LastName.Contains(stuname)
                             select new CombineModels
                       {
                           Exhibitions = e,
                           Designs = d,
                           Students = stu
                       };
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
                return RedirectToAction("Login", "Home");
            }
            else
            {
                //Viewbag list of student designs
                ViewBag.designList = designStudentList();
                return View();
            }
        }

        public IActionResult Courses() //Index of Courses page
        {
            return View();
        }

        public IActionResult AboutUs() //Index of AboutUs page
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string accName, string accPass)
        {
            try
            {
                var staff = db.Staff.SingleOrDefault(s => s.Email.Equals(accName)); //check account name of staff
                var student = db.Student.SingleOrDefault(s => s.Email.Equals(accName)); //check account name of student

                if (staff != null) //if account is staff
                {
                    var key = "b14ca5898a4e4133bbce2ea2315a1916";
                    staff.Password = AesEncDesc.DecryptString(key, staff.Password); //decrypt password
                    if (staff.Password.Equals(accPass))
                    {
                        HttpContext.Session.SetString("ename", accName); //get account name from session
                        HttpContext.Session.SetString("staffName", staff.StaffName); //get staff name from session
                        HttpContext.Session.SetString("staffId", staff.StaffId); //get staff id from session
                        HttpContext.Session.SetString("staffImage", staff.ProfileImage); //get Staff image from session
                        HttpContext.Session.SetInt32("staffRole", staff.Role); //get Staff role from session
                        return RedirectToAction("Index", "Admin", new { area = "" }); //return to Index of Admin
                    }
                    else
                    {
                        ViewBag.Msg = "Wrong Email or Pasword...."; //error password message
                    }
                }
                else if (student != null) //if account is student
                {
                    var key = "b14ca5898a4e4133bbce2ea2315a1916";
                    student.Password = AesEncDesc.DecryptString(key, student.Password);
                    if (student.Password.Equals(accPass))
                    {
                        HttpContext.Session.SetString("ename", student.FirstName + " " + student.LastName); //get student name from session
                        HttpContext.Session.SetString("studentid", student.StudentId); //get student id from session
                        HttpContext.Session.SetString("studentImage", student.ProfileImage); //get student profile image from session
                        return RedirectToAction("Index", "Home"); //return to Index of Home
                    }
                    else
                    {
                        ViewBag.Msg = "Wrong Email or Pasword....";
                    }
                }
                else
                {
                    ViewBag.Msg = "Wrong Email or Pasword....";
                }
            }
            catch (Exception e)
            {
                ViewBag.Msg = e.Message; //error message
            }
            return View();
        }

        public IActionResult Logout() //logout
        {
            var model = HttpContext.Session.GetString("ename"); //get account name from session
            if (model != null)
            {
                HttpContext.Session.Clear(); //clear session
                return RedirectToAction("Index", "Home");

            }
            return RedirectToAction("Index", "Home");
        }
    }
}
