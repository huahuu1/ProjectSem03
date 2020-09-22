using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using ProjectSem03.Models;

namespace ProjectSem03.Controllers
{
    public class AdminController : Controller
    {
        //connection to database
        ProjectDB db;
        public AdminController(ProjectDB db)
        {
            this.db = db;
        }

        [DefaultBreadcrumb("Home")]
        public IActionResult Index()
        {
            //Count data in models
            ViewBag.countStaff = db.Staff.Count();
            ViewBag.countStudent = db.Student.Count();
            ViewBag.countCompetition = db.Competition.Count();
            ViewBag.countExhibition = db.Exhibition.Count();
            return View();
        }
    }
}
