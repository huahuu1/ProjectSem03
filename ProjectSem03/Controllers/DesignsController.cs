using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//step 1
using ProjectSem03.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.AspNetCore.HttpOverrides;
namespace ProjectSem03.Controllers
{
    public class DesignsController : Controller
    {
        //step 2
        ProjectDB db;
        public DesignsController(ProjectDB db)
        {
            this.db = db;
        }
        //INDEX STAFF Foward best design to Exhibition
        public IActionResult Index(string searchName, string dropdownlistname)
        {
            //if (HttpContext.Session.GetString("staffname") == null) //check session
            //{
            //    return RedirectToAction("Login");
            //}
            //else
            //{
                var list = from d in db.Design
                           join s in db.Student on d.StudentId equals s.StudentId into table1
                           from s in table1.DefaultIfEmpty()
                           join p in db.Posting on d.DesignId equals p.DesignID into table2
                           from p in table2.DefaultIfEmpty()
                           select new DesignStudentPosting
                           {
                               Design = d,
                               Student = s,
                               Posting = p
                           };
                List<string> mark = new List<string>();
                mark.Add("best");
                mark.Add("better");
                mark.Add("good");
                mark.Add("moderate");
                mark.Add("normal");
                mark.Add("disqualified");
                mark.Add(null);
                // Tạo SelectList
                SelectList markList = new SelectList(mark);

                // Set vào ViewBag
                ViewBag.MarkList = markList;
                if (String.IsNullOrEmpty(searchName) && String.IsNullOrEmpty(dropdownlistname))
                {

                    return View(list);
                }
                else if (!String.IsNullOrEmpty(dropdownlistname) && !String.IsNullOrEmpty(searchName))
                {
                    var res = list.Where(d => d.Posting.Mark.Equals(dropdownlistname) && d.Design.DesignName.Contains(searchName));
                    return View(res);
                }
                else if (!String.IsNullOrEmpty(dropdownlistname))
                {
                    var res = list.Where(d => d.Posting.Mark.Equals(dropdownlistname));
                    return View(res);
                }
                else
                {
                    var res = list.Where(d => d.Design.DesignName.Contains(searchName));
                    return View(res);
                }
            //}//END check session
        }

        //UPDATE
        public IActionResult Forward(int id)
        {
            HttpContext.Session.SetInt32("designId", id);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Forward(Display display)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    display.Price = 0;
                    display.SoldStatus = false;
                    display.PaidStatus = false;
                    display.DesignID = (int)HttpContext.Session.GetInt32("designId");
                    db.Display.Add(display);
                    db.SaveChanges();                    
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Msg = "Fail";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Msg = ex.Message;
            }
            return View();
        }//End Forward
    }
}
